@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code
<style>

    .dropbox {
        /*width: 13em;*/
        height: 3em;
        border: 2px solid #DDD;
        border-radius: 8px;
        background-color: #FEFFEC;
        text-align: center;
        color: #BBB;
        font-size: 2em;
        font-family: Arial, sans-serif;
    }

        .dropbox span {
            margin-top: 0.9em;
            display: block;
        }

        .dropbox.not-available {
            background-color: #F88;
        }

        .dropbox.over {
            background-color: #bfb;
        }


    .loader {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: transparent;
        filter: alpha(opacity=50);
        /*-moz-opacity: 0.5;
            -khtml-opacity: 0.5;*/
        opacity: 0.8;
        z-index: 11000;
    }

    .centerloader {
        z-index: 1000;
        margin: 300px auto;
        padding: 10px;
        width: 105px;
        background-color: White;
        border-radius: 10px;
        filter: alpha(opacity=100);
        opacity: 1;
        -moz-opacity: 1;
    }

    #borderLayout_eGridPanel input[type="checkbox"] {
        appearance: none;
        -moz-appearance: none;
        -webkit-appearance: none;
        border: solid #5f5e5e 1px;
        width: 12px;
        height: 12px;
        transition: all 0.5s;
        cursor: pointer;
    }

    #borderLayout_eGridPanel input[type="checkbox"]:disabled {
        opacity: 0.5;
    }

    #borderLayout_eGridPanel input[type="checkbox"]:checked::after {
        content: '';
        display: block;
        width: 4px;
        height: 7px;
        border: solid #242424;
        border-width: 0px 2px 2px 0px;
        transform: rotate(45deg);
        margin-left: 3px;
        margin-top: 1px;
    }

        #borderLayout_eGridPanel input[type="checkbox"]:focus {
            outline: none;
        }

</style>

<div id="loadingDiv" class="loader" style="display: none">
    <div class="centerloader">
        <img alt="" src="~/images/spinner.gif" style="width:85px" />
    </div>
</div>

<script type="text/ng-template" id="modalPermissionDialogId"> 
    <h4 class="modal-title model-heading">Permissions for: {{folderOrFileName}}</h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <div class="col-md-12 no-padding">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <button class="btn btn-xs pull-right" ng-show="isEditable" ng-click="AddUserOrGroupRow()"><i class="fa fa-plus" title="Add new row"></i> Add User or Group</button>
                        <div class="clearfix"></div>
                        <div class="ag-blue space15 no-ag-cell-focus" ag-grid="usersAndGroupsOptions" style="width: 100%; height: 150px;"></div>
                        <div class="ag-blue space15 no-ag-cell-focus" ng-show="userOrGroupPermissions" ag-grid="userOrGroupPermissionsOptions" style="width: 100%; height: 150px;"></div>
                    </div>
                </div>
            </div>
        </div>
        <input type="button" class="btn btn-default btn-sm pull-right" value="Cancel" ng-click="closeThisDialog('')" />
        <button class="btn btn-success btn-sm pull-right margin-rgt10" ng-show="isEditable" ng-click="Save()">Ok</button>
    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalUserOrGroupSelectionDialogId">
    <h4 class="modal-title model-heading">Select User or Group</h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <div class="col-md-12 no-padding">
                <div class="panel panel-default">
                    <div class="panel-body">
                        Enter the object name to search: <input type="text" ng-model="userOrGroupName" ng-change="userOrGroupName.length <= 0 ? usersAndGroups = [] : false" style="width: 95px;" />
                        <button class="btn btn-xs" ng-disabled="userOrGroupName.length <= 0" ng-click="CheckNames()">Check Names</button>
                        <div class="ag-blue space15" ng-show="userOrGroupName.length > 0 && usersAndGroups.length > 0" ag-grid="usersAndGroupsOptions" style="width: 100%; height: 200px;"></div>
                    </div>
                </div>
            </div>
        </div>
        <input type="button" class="btn btn-default btn-sm pull-right" value="Cancel" ng-click="closeThisDialog('')" />
        <button class="btn btn-success btn-sm pull-right margin-rgt10" ng-disabled="userOrGroupName.length <= 0 || usersAndGroups.length <= 0 || !selectedUserOrGroup" ng-click="Save()">Ok</button>
    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalMetadataDialogId">
    <h4 class="modal-title model-heading">Metadata for: {{folderOrFileName}}</h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <div class="col-md-12 no-padding">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <button class="btn btn-xs pull-right" ng-show="isEditable" ng-click="AddTagRow()"><i class="fa fa-plus" title="Add new row"></i> Add Tag</button>
                        <div class="clearfix"></div>
                        <div class="ag-blue space15" id="tagList" ag-grid="tagOptions" style="width: 100%; height: 200px;"></div>
                    </div>
                </div>
            </div>
        </div>
        <input type="button" class="btn btn-default btn-sm pull-right" value="Cancel" ng-click="closeThisDialog('')" />
        <button class="btn btn-success btn-sm pull-right margin-rgt10" ng-show="isEditable" ng-click="Save()">Ok</button>
    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalAccountSelection">
    <h4 class="modal-title model-heading">Select Account </h4>
    <div class="model-bdy no-padding" ng-controller="AccountController as ctrl2">
        <div class="row no-padding">
            <div class="col-md-12 no-padding">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <form name="account">
                            @*<b>Select the account</b> userAccount: {{ctrl2.userAccount}}*@
                            @*<div style="border:solid 1px gray;"> </div>*@

                            <div style="overflow-y:auto; height:270px;">
                                <div ng-repeat="account in userAccounts">
                                    <label>
                                        <input type="radio" ng-model="ctrl2.userAccount" name="account" value="{{account.AccountId}}" required /> {{account.AccountName}}
                                    </label>
                                </div>
                            </div>

                            @*<select ng-model="ctrl2.userAccount" required>
                                    <option ng-repeat="account in user.userAccounts" value="{{account.accountId}}">{{account.accountName}}</option>
                                </select>
                                account.$invalid : {{account.$invalid}}*@
                            <br />
                        </form>
                    </div>
                </div>
            </div>
        </div>
        <input type="button" class="btn btn-default btn-sm pull-right" value="Cancel" ng-hide="!switchAccount" ng-click="closeThisDialog('')" />
        <button class="btn btn-success btn-sm pull-right margin-rgt10" ng-disabled="account.$invalid" ng-click="ctrl2.SetSelectedAccount(ctrl2.userAccount)">Ok</button>
    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalHistoryDialog">
    <h4 class="modal-title model-heading">History</h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <div class="col-md-12 no-padding">
                <div class="panel panel-default">
                    <div class="panel-body">
                        @*File id - {{id}}*@
                        <div class="ag-blue space15" id="HistoryList" ag-grid="HistoryDataOptions" style="width:100%; height: 200px;"></div>
                    </div>
                </div>
            </div>
        </div>
        <input type="button" class="btn btn-primary btn-sm pull-right" value="Close" ng-click="closeThisDialog(id)" />
    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalAlertDialog">
    <h4 class="modal-title model-heading">Alert </h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <div class="col-md-12 no-padding">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <div id="alertMessage" ng-class="alertClass" ng-bind-html="alertMessage"></div>
                    </div>
                </div>
            </div>
        </div>
        <input type="button" class="btn btn-primary btn-sm pull-right" value="Ok" ng-click="closeThisDialog('')" />

    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalFileUploadDialog">
    <h4 class="modal-title model-heading">File Upload in: {{folderName}}</h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <div class="col-md-12 no-padding">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <fieldset ng-disabled="isUploading">
                            <div>
                                <p>
                                    <strong>Select File To Upload</strong>:
                                    <br />
                                    <span class="input-control text">
                                        <input type="file" id="file" name="file" style="width: 52%" onchange="angular.element(this).scope().handleFileSelect(this)" />
                                    </span>
                                    <br />
                                    <div id="dropbox" class="dropbox" ng-class="dropClass"><span>{{dropText}}</span></div>
                                </p>
                                <div ng-show="selectedFile!=null">
                                    <div>
                                        <span>{{selectedFile.webkitRelativePath || selectedFile.name}} ({{(selectedFile.type|| 'Unknown')}})</span>
                                        (<span ng-switch="selectedFile.size > 1024*1024">
                                            <span ng-switch-when="true">{{selectedFile.size / 1024 / 1024 | number:2}} MB</span>
                                            <span ng-switch-default>{{selectedFile.size / 1024 | number:2}} KB</span>
                                        </span>)
                                        <br />
                                        <span ng-show="selectedFile.type!=null">File type - {{selectedFile.type}}</span>
                                    </div>

                                    <div ng-show="progressVisible==true" style="margin-bottom:5px;overflow: hidden;">
                                        <div class="percent">{{progress}}%</div>
                                        <div class="upload-progress-bar">
                                            <div class="uploaded" ng-style="{'width': progress+'%'}"></div>
                                        </div>
                                    </div>
                                    <input type="button" class="btn btn-success btn-sm" ng-click="btnUploadClick()" value="Upload" />
                                </div>

                            </div>
                        </fieldset>

                    </div>
                </div>

            </div>
        </div>
        <input type="button" class="btn btn-default btn-sm pull-right" value="Cancel" ng-disabled="isUploading" ng-click="closeThisDialog('')" />
    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalAddFolderDialog">
    <h4 class="modal-title model-heading">Create folder </h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <form name="createFolderForm">
                <div class="col-md-12 no-padding">
                    <div class="panel panel-default">
                        <div class="panel-body">
                            <div class="input-group input-group-sm ">
                                <span class="input-group-addon">
                                    Folder Name <span class="text-danger" ng-show="createFolderForm.name.$error.required">*</span>:
                                </span>
                                <input autofocus type="text" class="form-control" name="name"  ng-model="name" maxlength="100" ng-pattern="/^[\w\s-]+$/" required />
                            </div>
                        </div>
                    </div>

                </div>
            </form>
        </div>
        <input type="button" class="btn btn-default btn-sm pull-right" value="Cancel" ng-click="closeThisDialog('')" />
        <input type="button" class="btn btn-success btn-sm pull-right margin-rgt10" value="Create" ng-disabled="!createFolderForm.name.$valid" ng-click="createFolderForm.name.$valid && CreateFolderClick($event)" />

    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalRenameDialog">
    <h4 class="modal-title model-heading">Rename </h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <form name="renameForm">
                <div class="col-md-12 no-padding">
                    <div class="panel panel-default">
                        <div class="panel-body">
                            <div class="input-group input-group-sm ">
                                <span class="input-group-addon">
                                    Name <span class="text-danger" ng-show="renameForm.name.$error.required">*</span>:
                                </span>
                                <input type="text" class="form-control" name="name" ng-model="name" ng-pattern="folderPattern" required />
                            </div>
                        </div>
                    </div>

                </div>
            </form>
        </div>
        <input type="button" class="btn btn-default btn-sm pull-right" value="Cancel" ng-click="closeThisDialog('')" />
        <input type="button" class="btn btn-success btn-sm pull-right margin-rgt10" value="Modify" ng-disabled="!renameForm.name.$valid" ng-click="renameForm.name.$valid && RenameClick($event)" />

    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalMoveDialog">
    <h4 class="modal-title model-heading">Move: {{folderOrFileName}}</h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <div class="col-md-12 no-padding">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <div class="mobo-no-padding no-padding-left ag-file-browser ag-file-browser-smaller slow fadeIn animated" ag-grid="moveOptions" style="width:100%;"></div>
                    </div>
                </div>
            </div>
        </div>
        <input type="button" class="btn btn-default btn-sm pull-right" value="Cancel" ng-click="closeThisDialog('')" />
        <input type="button" class="btn btn-success btn-sm pull-right margin-rgt10" value="Move" ng-disabled="!selectedChildFolder || !selectedChildFolder.value" ng-click="Move()" />
    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalMoveDecisionDialog">
    <h4 class="modal-title model-heading">Replace or Rename file</h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <div class="col-md-12 no-padding">
                <div class="panel panel-default">
                    <div class="panel-body">
                        The destination already has a file named "{{selectedFile.name}}".
                    </div>
                </div>
            </div>
        </div>
        <input type="button" class="btn btn-default btn-sm pull-right" value="Cancel" ng-click="closeThisDialog('')" />
        <input type="button" class="btn btn-success btn-sm pull-right margin-rgt10" value="Rename" ng-click="Rename()" />
        <input type="button" class="btn btn-info btn-sm pull-right margin-rgt10" value="Replace" ng-click="Replace()" />
    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalMoveDecisionRenameDialog">
    <h4 class="modal-title model-heading">Rename & Move</h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <form name="renameForm">
                <div class="col-md-12 no-padding">
                    <div class="panel panel-default">
                        <div class="panel-body">
                            <div class="input-group input-group-sm">
                                <span class="input-group-addon">
                                    Name <span class="text-danger" ng-show="renameForm.name.$error.required">*</span>:
                                </span>
                                <input type="text" class="form-control" name="name" ng-model="name" ng-pattern="pattern" required />
                            </div>
                        </div>
                    </div>
                </div>
            </form>
        </div>
        <input type="button" class="btn btn-default btn-sm pull-right" value="Cancel" ng-click="closeThisDialog('')" />
        <input type="button" class="btn btn-success btn-sm pull-right margin-rgt10" value="Rename & Move" ng-disabled="!renameForm.name.$valid" ng-click="renameForm.name.$valid && RenameAndMove()" />
    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalDeleteRestorePurgeDialog">
    <h4 class="modal-title model-heading">{{action}}{{(isAdmin || isAccountAdmin) && action === 'Delete' ? "/Purge" : ""}}: {{folderOrFileName}}</h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <div class="col-md-12 no-padding">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <div ng-if="folderOrFileName === 'All Items'" >
                            Are you sure to {{action === "Purge" ? "permanently delete" : action.toLowerCase()}}{{(isAdmin || isAccountAdmin) && action === 'Delete' ? "/purge" : ""}} these items?
                        </div>
                        <div ng-if="folderOrFileName !== 'All Items'" >
                            Are you sure to {{action === "Purge" ? "permanently delete" : action.toLowerCase()}}{{(isAdmin || isAccountAdmin) && action === 'Delete' ? "/purge" : ""}} this {{isFolder ? "folder" : "file"}}?
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <input type="button" class="btn btn-default btn-sm pull-right" value="Cancel" ng-click="closeThisDialog('')" />
        <button class="btn btn-danger btn-sm pull-right margin-rgt10" ng-show="(isAdmin || isAccountAdmin) && action === 'Delete'" ng-click="Action('Purge')">Purge</button>
        <button class="btn btn-success btn-sm pull-right margin-rgt10" ng-click="Action(action)">{{action}}</button>
    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalAdvanceSearchDialog">
    <h4 class="modal-title model-heading">Advanced Search</h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <form name="searchForm">
                <div class="col-md-12 no-padding">
                    <div class="panel panel-default">
                        <div class="panel-body">
                            <div>
                                <div class="input-group input-group-sm ">
                                    <span class="input-group-addon">
                                        Search Text
                                        @*<span class="text-danger" ng-show="searchForm.SearchText.$error.required">*</span>*@:
                                    </span>
                                    <input type="text" class="form-control" name="SearchText" placeholder="Search" ng-model="Filter.SearchText" />
                                    <span class="input-group-addon" id="sizing-addon3" ng-click="AddTagRow($event)"><i class="fa fa-plus" title="Add new row"></i></span>
                                </div>
                            </div>
                            <div class="ag-blue space15" id="tagList" ag-grid="tagOptions" style="width: 100%; height: 200px;">
                            </div>
                        </div>
                    </div>
                </div>
            </form>
        </div>
        <input type="button" class="btn btn-default btn-sm pull-right" value="Cancel" ng-click="closeThisDialog('')" />
        <input type="button" class="btn btn-success btn-sm pull-right margin-rgt10" value="Search" ng-click="searchForm.SearchText.$valid && SearchClick($event)" />
    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalDeletedFilesDialog">
    <h4 class="modal-title model-heading">Deleted files</h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">

            <div class="col-md-12 no-padding">
                <div class="panel panel-default">
                    <div class="panel-body">

                        <div class="ag-blue space15" id="deletedFileList" ag-grid="deletedFileOptions" style="width: 100%; height: 300px;">
                        </div>

                    </div>
                </div>
            </div>

        </div>
        <input type="button" class="btn btn-default btn-sm pull-right" value="Cancel" ng-click="closeThisDialog('')" />
        <input type="button" class="btn btn-success btn-sm pull-right margin-rgt10" value="Ok" ng-click="DeleteFileGridActionClick('closeDialog', $event, '')" />
    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalLinkFileDialog">
    <h4 class="modal-title model-heading">Link file</h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <div class="col-md-12 no-padding">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <div class="ag-blue space15" id="linkFileList" ag-grid="linkFileOptions" style="width: 100%; height: 300px;"></div>
                    </div>
                </div>
            </div>
        </div>
        <input type="button" class="btn btn-default btn-sm pull-right" value="Cancel" ng-click="closeThisDialog('')" />
        <input type="button" class="btn btn-success btn-sm pull-right margin-rgt10" value="Link" ng-disabled="!selectedFileToLink" ng-click="LinkFile()" />
    </div>
    <div class="clearfix"></div>
</script>

<script type="text/ng-template" id="modalConfirmDialog">
    <h4 class="modal-title model-heading">{{titleText ? titleText : 'Confirm'}}</h4>
    <div class="model-bdy no-padding">
        <div class="row no-padding">
            <div class="col-md-12 no-padding">
                <div class="panel panel-default">
                    <div class="panel-body">
                        {{bodyText}}
                    </div>
                </div>
            </div>

        </div>

        <button class="btn btn-default btn-sm pull-right" ng-click="closeThisDialog()">Cancel</button>
        <button class="btn btn-primary btn-sm pull-right mr-5" ng-click="confirm()">Confirm</button>
    </div>
    <div class="clearfix"></div>
</script>

<div ng-controller="FileController as ctrl">
    <div>
        <div ng-if="isUserAuthentic" class="hide" ng-class="{'show': isUserAuthentic == true}">
            <nav class="navbar navbar-inverse navbar-margin">
                <div class="container-fluid-1">
                    <div class="navbar-header pull-left">
                        @*<button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#navbar" aria-expanded="false" aria-controls="navbar">
                            <span class="sr-only">Toggle</span>
                            <span class="icon-bar"></span>
                            <span class="icon-bar"></span>
                            <span class="icon-bar"></span>
                        </button>*@
                        <a class="navbar-brand logo-brand" href="" ng-click="fileNavigator.goTo(-1)"><img src="/Images/logo.png"></a>

                    </div>
                    <div class="pull-right mt-x-10 text-right">
                        <span class="padding-right10 sync-right"> File Minister</span>

                        <span>
                            <a style="cursor: pointer;" ng-click="buttonClick('logout', $event)" title="Logout">
                                <i class="glyphicon glyphicon-log-out padding-right10 font20 blue-clr" aria-hidden="true"></i>
                            </a>
                        </span>
                    </div>
                    <div class="clearfix"></div>
                    <div class="pull-right pt-x-10 gray-clr wel-txt">
                        <span class=""> Welcome <span class="text-bold"> {{userDetail.userName}}</span> [{{selectedAccount.accountName}}]</span>
                        <span>
                            <a style="cursor: pointer;" ng-click="ctrl.openAccountSelectionDialog(true)" title="switch account">
                                <i class="fa fa-exchange padding-right10 exchange-icon gray-clr" aria-hidden="true"></i>
                            </a>
                        </span>
                    </div>
                </div>
            </nav>
            <div style=" background-color: #f5f5f5; border: 1px solid #ccc; border-radius: 4px; margin-bottom: 20px; padding: 10px;" class="space15 btn-icon">
                <div class="pull-left new-menu-bar">
                    <button id="btn" class="btn btn-primary btn-sm" ng-disabled="!IsOnlyOneFileSelected() " ng-click="buttonClick('history', $event)" title="View history">
                        <i class="fa fa-history font17" aria-hidden="true"></i>
                    </button>
                    <button id="btn" class="btn btn-primary btn-sm" ng-click="buttonClick('refresh', $event)" title="Refresh contents">
                        <i class="fa fa-refresh font17"></i>
                    </button>
                </div>
                <div class="pull-left new-menu-bar">
                    <button id="btn" class="btn btn-primary btn-sm" ng-disabled="IsCheckedOut() || !IsWrite() || !IsOnlyOneFileSelected()" ng-click="buttonClick('checkout', $event)" title="Check out">
                        <i class="fa fa-hand-o-down font17"></i>
                    </button>
                    <button id="undobtn" class="btn btn-primary btn-sm" ng-disabled="!IsAuthorizedToUndoCheckedOut() " ng-click="buttonClick('undocheckout', $event)" title="Undo Check out">
                        <i class="fa fa-hand-o-up font17"></i>
                    </button>
                </div>
                <div class="menu-bar new-menu-bar pull-left">

                    <button id="btn" class="btn btn-primary btn-sm" ng-disabled="!IsFolderSelected()" ng-click="buttonClick('createfolder', $event)" title="Create Folder">
                        <i class="fa fa-folder  font17" style="position: relative;">
                            <i class="fa fa-plus-circle new-folder" aria-hidden="true"></i>
                        </i>
                    </button>
                    <button id="btn" class="btn btn-primary btn-sm" ng-disabled="!IsFolderSelected()" ng-click="buttonClick('uploadfile', $event)" title="Upload file">
                        <i class="fa fa-cloud-upload font17"></i>
                    </button>
                    <button id="btn" class="btn btn-primary btn-sm" ng-disabled="!IsFileSelected() || IsFileAndFolderSelected()" ng-click="buttonClick('downloadfile', $event)" title="Download file">
                        <i class="fa fa-cloud-download font17"></i>
                    </button>
                    
                </div>
                <div class="pull-left new-menu-bar">
                    <button id="btn" class="btn btn-primary btn-sm" ng-if="IsAdmin || IsAccountAdmin" ng-click="buttonClick('deletedFiles', $event)" title="View deleted files">
                        <i class="fa fa-bitbucket font17"></i>
                    </button>

                    <button id="btn" class="btn btn-primary btn-sm" ng-disabled="!IsFileORFolderSelected() || !IsWrite()" ng-click="buttonClick('deleteFile', $event)" title="Delete file">
                        <i class="fa fa-trash font17"></i>
                    </button>
                </div>
                <div class="pull-left new-menu-bar">
                    <button id="btn" class="btn btn-primary btn-sm" ng-disabled="!IsFileSelected() || IsFileAndFolderSelected() || !IsWrite()" ng-click="buttonClick('moveFile', $event)" title="Move File">
                        <i class="fa fa-arrows font17"></i>
                    </button>

                    <button id="btn" class="btn btn-primary btn-sm" ng-disabled="!IsFileSelected()" ng-click="buttonClick('setPermissions', $event)" title="Set Permissions">
                        <i class="fa fa-user font17"></i>
                    </button>

                    <button id="btn" class="btn btn-primary btn-sm" ng-disabled="!IsFileSelected()" ng-click="buttonClick('setMetaData', $event)" title="Set Tags">
                        <i class="fa fa-file font17"></i>
                    </button>

                </div>
                <div class="clearfix"></div>                
                <div class="space10">
                    @*
                        <div class="fileselect col-sm-6" id="selectedFile">Select a file below...</div>
                    *@
                    <div class="input-group input-group-sm col-sm-8 pull-left">
                        <input type="text" class="form-control" placeholder="Select a file below..." ng-model="SelectedPath" readonly />
                        <span class="input-group-addon" id="sizing-addon3"><a title="open parent folder" ng-click="buttonClick('openparentfolder', $event)"><i class="fa fa-arrow-up"></i></a></span>
                    </div>
                    <div class="input-group input-group-sm col-sm-4 pull-right search-box">
                        <input type="text" class="form-control FilterSearchText" placeholder="Search" ng-model="Filter.SearchText" ng-change="SearchChange()">
                        <span class="input-group-addon" id="sizing-addon3">
                            <a ng-hide="!Filter.SearchText" ng-click="buttonClick('clearsearch', $event)"><i class="glyphicon glyphicon-remove padding-right10"></i></a>
                            <a ng-click="FileSearch()"><i class="glyphicon glyphicon-search padding-right10"></i></a>
                            <a title="Advanced Search" ng-disabled="!IsFolderSelected()" ng-click="buttonClick('advancesearch', $event)"><i class="glyphicon glyphicon-new-window" aria-hidden="true"></i></a>
                        </span>
                    </div>
                    <div class="clearfix"></div>
                </div>
                @*
                    <div style="display:none;" class="ag-row ag-row-no-focus ag-row-odd ag-row-group ag-row-level-1 ag-row-group-contracted" context-menu="ctrl.menuOptions">
                    </div>
                *@
                <div class="clearfix"></div>
                <div class="space10">
                    <div class="clearfix"></div>
                    <div style="position:relative;">
                        <div class="main-wrap">
                            <input id="slide-sidebar" class="hidden-lg hidden-md hidden-sm" type="checkbox" role="button" />
                            <label class="bar hidden-lg hidden-md hidden-sm" for="slide-sidebar"><span><i onclick="collapsearrow(this)" class="fa fa-chevron-left chevron fa-chevron-right"></i></span></label>
                            <div class="sidebar1">
                                <div class="pull-left mobo-no-padding no-padding-left ag-file-browser slow fadeIn animated" id="exampleFileBrowser" ag-grid="gridOptions" style="width:100%;"></div>

                            </div>
                            <div class="portfolio">
                                <div class="mobo-no-padding no-padding-right no-padding-left ag-file-browser" id="mainFileBrowser" ag-grid="maingridOptions" context-menu="ctrl.gridPanelMenuOptions()" model="ctrl.getSelectedFolderModel()"></div>

                            </div>
                            <div class="clearfix"></div>
                        </div>
                    </div>
                    <div class="clearfix"></div>


                    <div id="sidebar" style="border:solid red 1px; float:left; display:none;">
                       
                        @*
                            <div id="split-bar" ng-mousedown="split_bar_mousedown($event)"></div>
                        *@
                        
                    </div>
                    @*
                        <div style="position:absolute; margin-top:-20px; z-index:9999; left:10px;">
                            <i aria-hidden="true" class="fa fa-chevron-left cursor" id="sidebar-hide-btn"></i>
                            <i aria-hidden="true" class="fa fa-chevron-right cursor" id="sidebar-show-btn"></i>
                        </div>
                    *@
                    <div id="main" style="border:solid blue 1px; display:none; float:left;">
                       
                    </div>
                </div>
                <div class="clearfix"></div>
            </div>
            <div class="clearfix"></div>
        </div>
        <div ng-if="!isUserAuthentic" class="navbar-margins hide" ng-class="{'show': !isUserAuthentic}">
            <div class="logo-header">
                <div class="container-fluid">

                    <div class="pull-left mobo-widthtab"><a href="/" class="link-underline"><h2 class="uni-logo"><img src="/Images/logo.png"></h2></a></div>
                    <div class="pull-right tagline mobo-widthtab">
                        <h2 class="pull-left blue-clr">File Minister</h2>
                        <div class="pull-right">
                            <span class="btn btn-primary btn-blue login-btn" ng-click="ctrl.GotoLogin()">Login</span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="navbar navbar-login-page hidden">
                <div class="container">
                    @*
                        <div class="navbar-header">
                            <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                                <span class="icon-bar"></span>
                                <span class="icon-bar"></span>
                                <span class="icon-bar"></span>
                            </button>
                            <a class="navbar-brand" href="#">File Minister</a>
                        </div>
                        <div class="navbar-collapse collapse">
                            <ul class="nav navbar-nav">
                                <li><a href="/Home/BuyApp">ss</a></li>
                            </ul>
                        </div>
                    *@

                </div>
            </div>
            <div class="body-content">
                <img src="/Images/Cloud-1024x536.jpg" class="img-responsive banner-img img-center">
            </div>
            @*<button class="btn btn-default" ng-click="ctrl.GotoLogin()">Login</button>*@
            <div class="clearfix"></div>
            @*<div class="" style="height:20px"></div>*@
            <div class="container">
                <footer>
                    <p class="mb-0">© @(DateTime.Today.Year) - CloudSync</p>
                </footer>
            </div>
        </div>
    </div>

</div>

<div class="clearfix">&nbsp;</div>
@section script
    <script type="text/javascript">
    /**/
    /**/
    /**/
    /**/
    /**/
    /**/
    /**/
    /**/
    /**/
    /**/
    /**/
    /**/
    Settings('', '@Me.ViewBag.authorityUrl', '@Me.ViewBag.redirectUrl', '@Me.ViewBag.clientId', '@Me.ViewBag.clientKey', '@Me.ViewBag.webWorkSpaceId');
/**/
/**/
/**/
/**/
/**/
/**/
/**/
/**/
/**/
/**/
/**/
/**/

        //$("#sidebar-hide-btn").click(function () {
        //    $('#exampleFileBrowser').hide(100);
        //    $('#mainFileBrowser').removeClass('col-sm-9');
        //    $('#mainFileBrowser').addClass('col-sm-12');
        //});

        //$("#sidebar-show-btn").click(function () {
        //    $('#exampleFileBrowser').show(100);
        //    $('#mainFileBrowser').removeClass('col-sm-12');
        //    $('#mainFileBrowser').addClass('col-sm-9');
        //});

        //var min = 200;
        //var max = 300;
        //var mainmin = 200;

        //$('#split-bar').mousedown(function (e) {
        //    e.preventDefault();
        //    $(document).mousemove(function (e) {
        //        e.preventDefault();
        //        var x = e.pageX - $('#sidebar').offset().left;
        //        if (x > min && x < max && e.pageX < ($(window).width() - mainmin)) {
        //            $('#sidebar').css("width", x);
        //            $('#main').css("margin-left", x);
        //        }
        //    })
        //});

        //$(document).mouseup(function (e) {
        //    $(document).unbind('mousemove');
        //});
    </script>
    
<script>
    function collapsearrow(x) {
        x.classList.toggle("fa-chevron-right");
    }
</script>
End Section