Imports System.Text
Imports System.ComponentModel

Public Class EntityPaster

    Public Delegate Function DynamicLookupDelegate(ent As IDataEntity, gen_value As Object) As BindingList(Of KeyValueEntity)

    Protected _entityList As IEnumerable(Of IEntity)
    Protected _pasteErrorMap As New EntityPasteErrorMap

    Protected _fieldNames As List(Of String)
    'Backout
    Protected _dynamicLookupfieldNames As Dictionary(Of String, DynamicLookupDelegate)

    Protected _entityFieldData As List(Of List(Of String))

    Public Sub New(entityList As IEnumerable(Of IEntity), fieldNames As IEnumerable(Of String), pasteText As String, Optional dynamicLookupfieldNames As Dictionary(Of String, DynamicLookupDelegate) = Nothing)
        Me.New(entityList, fieldNames, ParseMultiRowPasteText(pasteText), dynamicLookupfieldNames)
    End Sub

    Public Sub New(entityList As IEnumerable(Of IEntity), fieldNames As IEnumerable(Of String), entityFieldData As List(Of List(Of String)), Optional dynamicLookupfieldNames As Dictionary(Of String, DynamicLookupDelegate) = Nothing)
        _entityList = entityList
        _fieldNames = fieldNames

        'Backout
        _dynamicLookupfieldNames = dynamicLookupfieldNames

        'Backout()
        If _dynamicLookupfieldNames Is Nothing Then
            _dynamicLookupfieldNames = New Dictionary(Of String, DynamicLookupDelegate)
        End If

        _entityFieldData = entityFieldData
    End Sub

    Public Overridable Sub Paste(Optional checkBusinessRule As Boolean = True)
        _pasteErrorMap.Clear()

        'translate static lookup field
        Dim translatedFieldData As List(Of List(Of String)) = TranslateLookupData(_entityFieldData)

        'do the paste by calling entity's BulkSetFields in a loop
        Dim total As Long = Math.Min(_entityList.Count, translatedFieldData.Count)

        For i As Integer = 0 To total - 1
            Dim ent As IDataEntity = _entityList(i)
            Dim dataLine As List(Of String) = translatedFieldData(i)

            Dim errorFields As FieldMessageMap = Nothing

            For j As Integer = 0 To Math.Min(_fieldNames.Count, dataLine.Count) - 1
                Dim fieldName As String = _fieldNames(j)
                Dim fieldValue As Object = dataLine(j)

                'Backout
                If _dynamicLookupfieldNames.ContainsKey(fieldName) And Not fieldValue = "" Then
                    'translate dynamic lookup field
                    fieldValue = GetDynamicValue(fieldName, ent, fieldValue)
                End If

                Try
                    ' If (fieldName <> "AdminSortGroup1" And fieldName <> "AdminSortGroup2" And fieldName <> "AdminSortGroup3") Then
                    ent.SetField(fieldName, fieldValue)
                    'End If

                    ' ent.SetField(fieldName, fieldValue)
                Catch ex As Exception
                    If errorFields Is Nothing Then
                        errorFields = New FieldMessageMap
                    End If

                    errorFields.AddFieldMessage(fieldName, ex.ToString)
                End Try
            Next

            If errorFields IsNot Nothing Then
                _pasteErrorMap.AddErroFieldList(ent, errorFields.FieldMessageList)
            End If
        Next

        If checkBusinessRule Then
            AddBusinessRuleError()
        End If
    End Sub

    'Backout
    Private Function GetDynamicValue(fieldName As String, ent As IDataEntity, originalValue As Object) As Object
        Dim kvList As BindingList(Of KeyValueEntity) = _dynamicLookupfieldNames(fieldName).Invoke(ent, originalValue)

        'User might put either key or display value in the field

        'First try match display text
        For Each kv As KeyValueEntity In kvList
            If Object.Equals(kv.Value, originalValue) Then
                Return kv.Key
            End If
        Next

        'If not found, try match key 
        For Each kv As KeyValueEntity In kvList
            If String.Equals(kv.Key.ToString, originalValue) Then
                Return kv.Key
            End If

            If Object.Equals(kv.Key, originalValue) Then
                Return kv.Key
            End If


        Next

        _pasteErrorMap.AddErrorField(ent, fieldName, "value not in lookup list")

        Return Nothing
    End Function

    Protected Overridable Function TranslateLookupData(src As List(Of List(Of String))) As List(Of List(Of String))
        'Default no translate, subclasses should implement this as needed
        Return src
    End Function

    Protected Sub AddBusinessRuleError()
        For Each ent As IDataEntity In _entityList
            If Not ent.CheckBusinessRules() Then
                If ent.BusinessRuleErrorList IsNot Nothing Then
                    _pasteErrorMap.AddErroFieldList(ent, ent.BusinessRuleErrorList)
                End If
            End If
        Next
    End Sub

    Public Function PasteDataLineCount() As Long
        If _entityFieldData IsNot Nothing Then
            Return _entityFieldData.Count
        End If

        Return 0
    End Function

    Public ReadOnly Property PasteErrorMap As EntityPasteErrorMap
        Get
            Return _pasteErrorMap
        End Get
    End Property

    'This is painful but the ultragrid paste does not detect multi-line text in one cell
    'hence we implement this parse function 
    Public Shared Function ParseMultiRowPasteText(text As String) As List(Of List(Of String))
        Dim rows As New List(Of List(Of String))
        Dim row As New List(Of String)
        Dim curField As New StringBuilder()

        Dim rowAdded As Boolean = False

        Dim previousChar As Char = Nothing
        Dim curChar As Char = Nothing
        Dim escaped As Boolean = False

        For i As Integer = 0 To text.Length - 1
            'new char found, so mark rowAdded as false
            rowAdded = False

            curChar = text.Chars(i)

            If escaped And (curChar = vbTab And previousChar = """") Then
                'End of escap
                'remove last "
                curField.Remove(curField.Length - 1, 1)
                escaped = False
            End If

            If curChar = vbLf And (Not escaped) Then
                'end of row reached
                If previousChar = vbCr Then
                    'remove last char if it's vbCr
                    curField.Remove(curField.Length - 1, 1)
                End If
                row.Add(curField.ToString)
                curField.Clear()

                rows.Add(row)
                rowAdded = True

                row = New List(Of String)
            ElseIf curChar = vbTab And (Not escaped) Then
                'end of field
                row.Add(curField.ToString())

                curField.Clear()
                escaped = False
            ElseIf curField.Length = 0 And curChar = """" Then
                'first char in field is "
                'it's a multi-line text
                'any tab or vbLf won't count as separator
                escaped = True
            Else
                curField.Append(curChar)
            End If

            previousChar = curChar
        Next

        If Not rowAdded Then
            row.Add(curField.ToString)
            rows.Add(row)
        End If

        Return rows
    End Function

    Public Function CheckPasteColumnNumberMatch() As Boolean
        For Each rowdata In _entityFieldData
            ' _fieldNames.Count - 3 because there are 3 admin fields at the end of the 
            'MASS_LOAD grid that connot be loaded from the spreadsheet
            'so we dont count those
            If rowdata.Count <> _fieldNames.Count Then
                Return False
            End If
        Next

        Return True
    End Function

    Public Function CheckPasteRowNumberMatch() As Boolean
        Return _entityFieldData.Count = _entityFieldData.Count
    End Function

End Class
