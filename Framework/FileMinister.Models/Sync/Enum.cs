
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
namespace FileMinister.Models.Enums
{

	

	public enum MainOptions
	{
		[Description("Switch User")]
		SwithUser = 2,
		[Description("Switch Account")]
		SwitchAccount = 3,
		[Description("Add Agent")]
		AddAgent = 4
	}

	public enum PermissionType : byte
	{
		[Description("Read")]
		Read = 1,
		[Description("Write")]
		Write = 2,
		[Description("Edit Tag")]
		Tag = 4,
		[Description("Share Admin")]
		ShareAdmin = 16,
		List = 8
	}

	public enum CommandType
	{
		None = 0,
		Refresh = 1,
		CheckIn = 2,
		CheckOut = 4,
		UndoCheckOut = 8,
		ResolveConflictMine = 16,
		ResolveConflictTheirs = 32,
		History = 64,
		NewFolder = 128,
		Cut = 256,
		Copy = 512,
		Paste = 1024,
		Delete = 2048,
		OpenFolderInExplorer = 4096,
		Properties = 8192,
		Link = 16384,
		UndoDelete = 32768,
		GetLatest = 65536,
		ResolveUsingServer = 131072,
		ResolveUsingUser = 262144,
		RequestUserfile = 524288,
		CloudSettings = 1048576,
		CloudUI = 2097152,
		TrayExit = 4194304,
		SaveUploadedConflict = 8388608
	}

	public enum Status
	{
		None = 0,
		Success = 200,
		Error = 500,
		NotFound = 404,
		AlreadyExists = 2,
		FileCheckedOutByDifferentUser = 3,
		AccessDenied = 4,
		VersionNotLatest = 5,
		PermanentlyDeleted = 6,
		FileCheckedOut = 7,
		FileOpen = 8,
		PathNotValid = 10,
		LinkedFileNotDeleted = 11,
		LinkDeleted = 12,
		ChildNotDeleted = 13

	}


	public enum UserGroupType
	{
		User = 1,
		Group = 2
	}

	public enum FileEntryStatus : byte
	{
		NoActionRequired = 0,
		PendingDownload = 1,
		Downloading = 2,
		Downloaded = 3,
		MovingDownloaded = 4,
		NewModified = 5,
		PendingUpload = 6,
		Uploading = 7,
		PendingCheckInAfterUploading = 8,
		PendingFileSystemEntryDelete = 9,
		MoveOrRename = 10,
		TempCreated = 11,
		NoDownloadRequired = 12,
		NotFoundOnAzureStorage = 13
	}

	//Public Enum FileMoveNotifier
	//    NotRequired = 0
	//    PendingConfirm = 1
	//    Confirmed = 2
	//End Enum

	public enum FileVersionConflictType : byte
	{
		Modified = 1,
		ServerDelete = 2,
		ClientDelete = 3,
		InTheWay = 4,
		ClientModifyServerRename = 5
	}

	public enum ConflictUploadStatus : byte
	{
		Requested = 1,
		Uploading = 2,
		Uploaded = 3
	}

	public enum SharedAccessSignatureType
	{
		Download = 0,
		Upload = 1
	}

    public enum ConflictResolutonMode
    {
        UseCurrent = 1,
        UseSpecific = 2
    }

    public enum CheckInSource
    {
        Web = 1,
        Cloud = 2
    }

    public enum UserRole
    {
        AccountAdmin = 1,
        Normal = 2
    }
}

