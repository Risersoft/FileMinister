declare var min: any;
declare var max: any;
declare var mainmin: any;
declare class FileControllerGrid {
    FileService: IFileService;
    ngDialog: angular.dialog.IDialogService;
    OrganizationService: IOrganizationService;
    PermissionService: IPermissionService;
    TagService: ITagService;
    $scope: $Scope;
    $rootScope: $RootScope;
    $timeout: ng.ITimeoutService;
    $compile: ng.ICompileService;
    $filter: ng.IFilterService;
    authInterceptorService: authInterceptorService;
    constructor(FileService: IFileService, ngDialog: angular.dialog.IDialogService, OrganizationService: IOrganizationService, PermissionService: IPermissionService, TagService: ITagService, $scope: $Scope, $rootScope: $RootScope, $timeout: ng.ITimeoutService, $compile: ng.ICompileService, $filter: ng.IFilterService, authInterceptorService: authInterceptorService);
    ReGenerateMainGridData(filter: any): void;
    InnerCellRenderer: (params: any) => string;
    SetGridFocusedRow: () => void;
    LoadFileExplorer(): void;
    menuOptionsTree: (folderOrShare: any) => ({
        text: string;
        enabled: () => boolean;
        click: ($itemScope: any, $event: any, model: any) => void;
    } | {
        text: string;
        click: ($itemScope: any, $event: any, model: any) => void;
        enabled?: undefined;
    })[];
    menuOptions: (item: any) => ({
        text: string;
        enabled: () => any;
        click: ($itemScope: any, $event: any, model: any) => void;
    } | {
        text: string;
        click: ($itemScope: any, $event: any, model: any) => void;
        enabled?: undefined;
    })[];
    RefreshFileExplorer(): void;
    RefreshMaingrid(arr: any): void;
    GetFolderNode(key: any): any;
    GetFileNode(key: any): any;
    OpenFolder(key: any): void;
    OpenParentFolder(): void;
    RefreshGridsData(key: any, expanded: any): void;
    ClearRowSelection: (event: any) => void;
    DownloadFile(selectedRow: any): void;
    DownloadDeletedFile(selectedRow: any): void;
}
declare class FileControllerDialogs extends FileControllerGrid {
    FileService: IFileService;
    ngDialog: angular.dialog.IDialogService;
    OrganizationService: IOrganizationService;
    PermissionService: IPermissionService;
    TagService: ITagService;
    $scope: $Scope;
    $rootScope: $RootScope;
    $timeout: ng.ITimeoutService;
    $compile: ng.ICompileService;
    $filter: ng.IFilterService;
    authInterceptorService: authInterceptorService;
    constructor(FileService: IFileService, ngDialog: angular.dialog.IDialogService, OrganizationService: IOrganizationService, PermissionService: IPermissionService, TagService: ITagService, $scope: $Scope, $rootScope: $RootScope, $timeout: ng.ITimeoutService, $compile: ng.ICompileService, $filter: ng.IFilterService, authInterceptorService: authInterceptorService);
    openPermissionDialog(model: any): void;
    openTagDialog(model: any): void;
    openAccountSelectionDialog(switchAccount: boolean): void;
    syncData(): void;
    OpenFileUploadDialog(): void;
    OpenCreateFolderDialog(selectedNode?: any): void;
    OpenRenameDialog: (fileEntryId: any, versionNumber: any, newName: string, folder: any) => void;
    OpenMoveDialog: (model: any) => void;
    DeleteRestorePurgeDialog: (model: any, action: string, selectedNode?: any) => void;
    OpenConfirmDialog: (bodyText: string, titleText: string, callback: any) => void;
    OpenDeletedFileDialog: () => void;
    OpenAdvanceSearchDialog(): void;
}
declare class FileController$Scope extends FileControllerDialogs {
    FileService: IFileService;
    ngDialog: angular.dialog.IDialogService;
    OrganizationService: IOrganizationService;
    PermissionService: IPermissionService;
    TagService: ITagService;
    $scope: $Scope;
    $rootScope: $RootScope;
    $timeout: ng.ITimeoutService;
    $compile: ng.ICompileService;
    $filter: ng.IFilterService;
    authInterceptorService: authInterceptorService;
    constructor(FileService: IFileService, ngDialog: angular.dialog.IDialogService, OrganizationService: IOrganizationService, PermissionService: IPermissionService, TagService: ITagService, $scope: $Scope, $rootScope: $RootScope, $timeout: ng.ITimeoutService, $compile: ng.ICompileService, $filter: ng.IFilterService, authInterceptorService: authInterceptorService);
    ClearSearch(): void;
    Logout(): void;
    RefreshPage(): void;
}
declare class FileController extends FileController$Scope implements angular.IController {
    FileService: IFileService;
    ngDialog: angular.dialog.IDialogService;
    OrganizationService: IOrganizationService;
    PermissionService: IPermissionService;
    TagService: ITagService;
    $scope: $Scope;
    $rootScope: $RootScope;
    $timeout: ng.ITimeoutService;
    $compile: ng.ICompileService;
    $filter: ng.IFilterService;
    authInterceptorService: authInterceptorService;
    static $: string[];
    $onInit(): void;
    constructor(FileService: IFileService, ngDialog: angular.dialog.IDialogService, OrganizationService: IOrganizationService, PermissionService: IPermissionService, TagService: ITagService, $scope: $Scope, $rootScope: $RootScope, $timeout: ng.ITimeoutService, $compile: ng.ICompileService, $filter: ng.IFilterService, authInterceptorService: authInterceptorService);
    getSelectedFolderModel: () => any;
    gridPanelMenuOptions: () => any;
    GotoLogin(): void;
    AttachDropzone(): void;
    setAccount(): void;
}
