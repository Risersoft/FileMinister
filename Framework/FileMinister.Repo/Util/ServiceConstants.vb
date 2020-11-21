Imports risersoft.shared.cloud

Public Class ServiceConstants

    Public Const OPERATION_SUCCESSFUL As String = "Operation Successful"
    Public Const NOTHAVE_APPROPRIATEPERMISSION As String = "User doesn't have appropriate permission"
    Public Const FILE_NOTFOUND As String = "File not found"
    Public Const FILEPATH_NOLONGERVALID As String = "File path no longer valid on server"
    Public Const FOLDERFILE_RESTORED As String = "The {0} is restored successfully."
    Public Const FILE_VERSION_MISMATCHED As String = "File version mismacthed"
    Public Const FILEFOLDER_ALREADY_EXISTS As String = "File/Folder Already Exists"
    Public Const NOTHAVE_UPLOAD_FILEPERMISSIONS As String = "You do not have permission to Upload the file"
    Public Const NOTHAVE_TASKPERMISSIONS As String = "You do not have Permission to perform the task"
    Public Const UNABLE_GET_BLOBREFERENCE As String = "Unable to get block blob reference for FileEntry with Id '{0}' and FileShare with Id '{1}'."
    Public Const UNABLE_CREATE_SAS As String = "Unable to create '{0}' type shared access signature for '{1}' blob of FileShare with Id '{2}'."
    Public Const NULL_STORAGECONNECTION_OR_CONTAINERNAME As String = "Fetched StorageConnectionString or ContainerName for FileEntry with Id '{0}' is null."
    Public Const NULL_STORAGECONNECTION_OR_SHARENAME As String = "Fetched StorageConnectionString or ShareName for FileEntry with Id '{0}' is null."
    Public Const NOTHAVE_DOWNLOAD_FILEPERMISSION As String = "You do not have permission to download the file"
    Public Const UNABLE_GET_BLOCKBLOBREFERENCE As String = "Unable to get block blob reference for '{0}' blob of FileShare with Id '{1}'."
    Public Const FILE_ALREADY_CHECKOUT As String = "File checked out by a different user {0}"
    Public Const ATLEAST_FILEFOLDER_NOT_DELETED As String = "At least One child file/folder is not deleted"
    Public Const FILE_NOT_OPEN As String = "File is not Open"
    Public Const FILE_ALREADY_OPENED As String = "File is already Open"
End Class

'Public Class AuthData
'    Public Shared Property Police As IAuthPolice
'    Public Shared Property User As LocalWorkSpaceInfo

'    Public Shared DataServiceUrl As String
'    Public Shared SyncServiceUrl As String
'    Public Shared FileWebApiUrl As String
'    Public Shared AuthorityUrl As String
'End Class