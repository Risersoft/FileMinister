
interface $RootScope extends ng.IRootScopeService {
    userDetail: any;
    userAccounts: any[];
    selectedAccount: any;
    setUser(user: any): any;
    getUser(): any;
    setSelectedAccount(account: any): any;
    IsAccountAdmin: boolean;
    isUserAuthentic: boolean;
    accountDetails: any;
}

interface $Scope extends ng.IScope {
    tags: any[],
    gridOptions: any,
    maingridOptions: any,
    showContextmenu(event: any, attrs: any): void,
    contextAttrs: any,
    UserShares: any[],
    SelectedFolder: string,
    split_bar_mousedown(e: any): void,
    openHistoryDialog(id: string): void,
    UndoCheckout(): void,
    CheckOut(): void,
    ShowAlert(msg: string, alertType: any): void,
    buttonClick(key: string, e: any): void,
    IsFileSelected(): boolean,
    IsFileORFolderSelected(): boolean,
    IsFolderSelected(): boolean,
    FileSearch():any,
    Filter: any,
    searchChanged: boolean,
    IsAdmin: boolean,
    IsShareSelected: any,
    IsOnlyOneFileSelected: any,
    IsFileAndFolderSelected: any,
    IsWrite: any,
    IsCheckedOut: any,
    IsAuthorizedToUndoCheckedOut: any,
    IsBlobContainerSelected: any,
    SearchChange: any,
    switchAccount:boolean
}