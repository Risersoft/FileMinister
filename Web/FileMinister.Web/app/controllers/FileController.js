var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var min = 200;
var max = 300;
var mainmin = 200;
//All grid configs, grid binding, grid refresh, context menus settings
var FileControllerGrid = /** @class */ (function () {
    function FileControllerGrid(FileService, ngDialog, OrganizationService, PermissionService, TagService, $scope, $rootScope, $timeout, $compile, $filter, authInterceptorService) {
        this.FileService = FileService;
        this.ngDialog = ngDialog;
        this.OrganizationService = OrganizationService;
        this.PermissionService = PermissionService;
        this.TagService = TagService;
        this.$scope = $scope;
        this.$rootScope = $rootScope;
        this.$timeout = $timeout;
        this.$compile = $compile;
        this.$filter = $filter;
        this.authInterceptorService = authInterceptorService;
        this.InnerCellRenderer = function (params) {
            var image;
            if (params.node.group) {
                image = params.node.level === 0 ? 'disk' : 'folder';
            }
            else {
                image = 'file';
            }
            var imageFullUrl = '/Images/' + image + '.png';
            var value = '<img src="' + imageFullUrl + '" style="padding-left: 4px;" /> ' + params.data.name;
            return value;
        };
        this.SetGridFocusedRow = function () {
            var that = this;
            var rows = $(that.$scope.gridOptions.api.gridPanel.eBodyViewport).find("div.ag-row");
            var selectedRowIndex = -1;
            for (var i = 0; i < rows.length; i++) {
                if ($(rows[i]).hasClass("ag-row-selected")) {
                    selectedRowIndex = i;
                    break;
                }
            }
            if (selectedRowIndex !== -1)
                that.$scope.gridOptions.api.setFocusedCell(selectedRowIndex, "name");
        };
        this.menuOptionsTree = function (folderOrShare) {
            var that = this;
            return [
                //Open Folder
                {
                    text: 'Open Folder',
                    enabled: function () {
                        return folderOrShare.folder === true;
                    },
                    click: function ($itemScope, $event, model) {
                        var data = model;
                        var key = model.value;
                        that.$scope["OpenFolderByDoubleClick"] = true;
                        that.OpenFolder(key);
                    }
                },
                null,
                {
                    text: 'Create Folder',
                    enabled: function () {
                        return folderOrShare.CanWrite === true;
                    },
                    click: function ($itemScope, $event, model) {
                        var data = model.value;
                        that.OpenCreateFolderDialog(folderOrShare);
                    }
                },
                null,
                {
                    text: 'Upload File',
                    enabled: function () {
                        return folderOrShare.CanWrite === true;
                    },
                    click: function ($itemScope, $event, model) {
                        var data = model.value;
                        that.OpenFileUploadDialog();
                    }
                },
                null,
                {
                    text: 'Delete',
                    enabled: function () {
                        return this.gridOptions.api.getSelectedRows().length > 0 && !folderOrShare.isShare && folderOrShare.CanWrite === true && folderOrShare.isDeleted !== true;
                    },
                    click: function ($itemScope, $event, model) {
                        that.DeleteRestorePurgeDialog(model, "Delete", folderOrShare);
                    }
                },
                //null, // Dividier for History
                //{
                //    text: 'History',
                //    enabled: function () {
                //        return !folderOrShare.isShare;
                //    },
                //    click: function ($itemScope, $event, model) {
                //        var data = model.value;
                //        that.$scope.openHistoryDialog(data);
                //    }
                //},
                null,
                {
                    text: 'Permission',
                    click: function ($itemScope, $event, model) {
                        that.openPermissionDialog(model);
                    }
                },
                null,
                {
                    text: 'Metadata',
                    click: function ($itemScope, $event, model) {
                        that.openTagDialog(model);
                    }
                },
            ];
        };
        this.menuOptions = function (item) {
            var that = this;
            this.$scope.gridOptions.api.setFocusedCell();
            var node = that.GetFileNode(item.value);
            //if (that.$scope.maingridOptions.rowSelection === 'single')
            node.setSelected(true, true);
            //else
            //    node.setSelected(true);
            if (item.version <= 0)
                return [];
            return [
                {
                    text: 'Open Folder',
                    enabled: function () {
                        return item.folder === true;
                    },
                    click: function ($itemScope, $event, model) {
                        var data = model;
                        var key = model.value;
                        that.$scope["OpenFolderByDoubleClick"] = true;
                        that.OpenFolder(key);
                    }
                },
                //null, // Dividier
                //{
                //    text: 'Create Folder',
                //    enabled: function () {
                //        return item.CanWrite === true && !item.folder;
                //    },
                //    click: function ($itemScope, $event, model) {
                //        var data = model.value;
                //        that.OpenCreateFolderDialog();
                //    }
                //},
                null,
                {
                    text: 'Rename',
                    enabled: function () {
                        return this.maingridOptions.api.getSelectedRows()[0].version > 0 && item.CanWrite === true && item.folder !== true;
                    },
                    click: function ($itemScope, $event, model) {
                        var data = model.value;
                        that.OpenRenameDialog(model.value, model.version, model.name, model.folder);
                    }
                },
                null,
                {
                    text: 'Move',
                    enabled: function () {
                        return this.maingridOptions.api.getSelectedRows()[0].version > 0 && item.CanWrite === true && item.folder !== true;
                    },
                    click: function ($itemScope, $event, model) {
                        that.OpenMoveDialog(model);
                    }
                },
                null,
                {
                    text: 'Delete',
                    enabled: function () {
                        return this.maingridOptions.api.getSelectedRows()[0].version > 0 && item.CanWrite === true && item.isDeleted !== true;
                    },
                    click: function ($itemScope, $event, model) {
                        that.DeleteRestorePurgeDialog(model, "Delete");
                    }
                },
                //{
                //    text: 'Restore',
                //    enabled: function () {
                //        return this.maingridOptions.api.getSelectedRows()[0].version > 0 && item.CanWrite === true && item.isDeleted === true && that.$rootScope.userDetail && that.$rootScope.userDetail.IsAdmin === true;
                //    },
                //    click: function ($itemScope, $event, model) {
                //        that.DeleteRestorePurgeDialog(model, "Restore");
                //    }
                //},
                //{
                //    text: 'Purge',
                //    enabled: function () {
                //        return this.maingridOptions.api.getSelectedRows()[0].version > 0 && item.CanWrite === true && item.isDeleted === true && that.$rootScope.userDetail && that.$rootScope.userDetail.IsAdmin === true;
                //    },
                //    click: function ($itemScope, $event, model) {
                //        that.DeleteRestorePurgeDialog(model, "Purge");
                //    }
                //},
                null,
                {
                    text: 'History',
                    enabled: function () {
                        return this.IsFileSelected();
                    },
                    click: function ($itemScope, $event, model) {
                        var data = model.value;
                        this.openHistoryDialog(data);
                    }
                },
                null,
                {
                    text: 'Download',
                    enabled: function () {
                        return item.folder === false && item.version > 0; //this.IsFileSelected();
                    },
                    click: function ($itemScope, $event, model) {
                        var data = model.value;
                        this.buttonClick('downloadfile', $event);
                    }
                },
                null,
                {
                    text: 'Undo Checkout',
                    enabled: function () {
                        return item.folder === false && item.CanWrite === true && item.version > 0 && item.CheckedOutBy && (that.$rootScope.userDetail.userName === item.CheckedOutBy || that.$rootScope.IsAccountAdmin);
                    },
                    click: function ($itemScope, $event, model) {
                        var data = model.value;
                        this.UndoCheckout(data);
                    }
                },
                null,
                {
                    text: 'Check-out',
                    enabled: function () {
                        return item.folder === false && item.version > 0 && (item.CheckedOutBy === null || item.CheckedOutBy === "") && item.CanWrite === true; //this.IsFileSelected();
                    },
                    click: function ($itemScope, $event, model) {
                        var data = model.value;
                        this.CheckOut();
                    }
                },
                null,
                {
                    text: 'Permission',
                    click: function ($itemScope, $event, model) {
                        that.openPermissionDialog(model);
                    }
                },
                null,
                {
                    text: 'Metadata',
                    click: function ($itemScope, $event, model) {
                        that.openTagDialog(model);
                    }
                }
                //null, // Dividier
                //['Properties', function ($itemScope) {
                //    // Code
                //}, [
                //        ['Permission', function ($itemScope, $event, model) {
                //            that.openPermissionDialog(model);
                //        }],
                //        null, // Dividier
                //        ['Metadata', function ($itemScope, $event, model) {
                //            that.openTagDialog(model);
                //        }]
                //    ]
                //],
            ];
        };
        this.ClearRowSelection = function (event) {
            var that = this;
            $(event.api.gridPanel.eBodyViewport).off().on("click", function (e) {
                if ($(e.target).hasClass("ag-body-viewport")) {
                    event.api.deselectAll();
                    event.api.refreshView();
                    that.$scope.gridOptions.api.setFocusedCell();
                }
            });
        };
    }
    FileControllerGrid.prototype.ReGenerateMainGridData = function (filter) {
        var that = this;
        var selectedRowsLeftGrid = that.$scope.gridOptions.api.getSelectedRows();
        if (!selectedRowsLeftGrid.length)
            return;
        var key = selectedRowsLeftGrid[0]["value"];
        //create filter params
        //filter = filter || {};        
        that.$scope.maingridOptions.columnApi.setColumnVisible('path', false);
        if (filter) {
            filter.StartFileId = key;
            that.$scope.maingridOptions.columnApi.setColumnVisible('path', true);
        }
        that.FileService.GetAllChildren({ id: key, filter: filter })
            .then(function (resp) {
            var allChildNodes = that.FileService.ConvertToMainGridModel(resp.data.Data);
            that.RefreshMaingrid(allChildNodes);
        }, function (resp) {
            //console.log(resp);
        });
    };
    FileControllerGrid.prototype.LoadFileExplorer = function () {
        var that = this;
        var rowData = this.$scope.UserShares;
        var sizeCellStyle = function () {
            return { 'text-align': 'right' };
        };
        var columnDefs = [
            {
                headerName: "", field: "name",
                cellRenderer: 'group',
                cellRendererParams: {
                    innerRenderer: that.InnerCellRenderer
                }
                //, minWidth: 50, maxWidth: 500
            }
        ];
        this.$scope.gridOptions = {
            columnDefs: columnDefs,
            rowData: rowData,
            rowSelection: 'single',
            enableColResize: false,
            enableSorting: false,
            angularCompileRows: true,
            rowHeight: 20,
            getNodeChildDetails: function (file) {
                if (file.folder) {
                    return {
                        group: true,
                        children: file.children,
                        expanded: file.open
                    };
                }
                else {
                    return null;
                }
            },
            icons: {
                groupExpanded: '<i class="fa fa-minus-square-o"/>',
                groupContracted: '<i class="fa fa-plus-square-o"/>'
            },
            onGridReady: function (event) {
                event.api.sizeColumnsToFit();
                $(event.api.gridPanel.eBodyViewport).off().on("click", function (e) {
                    if ($(e.target).hasClass("ag-body-viewport")) {
                        event.api.refreshView();
                        that.SetGridFocusedRow();
                        //clear row selection in right grid
                        that.$scope.maingridOptions.api.deselectAll();
                        that.$scope.maingridOptions.api.refreshView();
                    }
                });
            },
            onRowClicked: function (params) {
                that.OpenFolder(params.data.value);
            },
            onRowGroupOpened: function (e) {
                //that.$scope.gridOptions.api.showLoadingOverlay();               
                //e.node["groupExpandclicked"] = true;
            },
            // CALLBACKS
            processRowPostCreate: function (params) {
                $(params.eRow).attr("context-menu", "ctrl.menuOptionsTree(data)");
                $(params.eRow).attr("model", "data");
            },
            overlayNoRowsTemplate: '<span style="">No Shares.</span>'
        };
        var maincolumnDefs = [
            {
                headerName: "Name", field: "name", width: 250,
                cellRenderer: function (params) {
                    var image;
                    if (params.data.folder) {
                        image = 'folder';
                    }
                    else {
                        image = 'file';
                    }
                    var imageFullUrl = '/Images/' + image + '.png';
                    var value = '<img src="' + imageFullUrl + '" style="padding-left: 4px;" /> ' + params.data.name;
                    return value;
                }
            },
            { headerName: "Path", field: "path", width: 250 },
            { headerName: "Version", field: "version", width: 75, cellStyle: sizeCellStyle },
            { headerName: "Checked Out By", field: "CheckedOutBy", width: 150 },
            { headerName: "Last Checked In", field: "LastCheckedIn", width: 200 }
        ];
        this.$scope.maingridOptions = {
            // PROPERTIES 
            columnDefs: maincolumnDefs,
            rowData: null,
            rowSelection: 'multiple',
            enableColResize: true,
            enableSorting: true,
            rowHeight: 20,
            suppressRowClickSelection: false,
            angularCompileRows: true,
            suppressLoadingOverlay: true,
            suppressCellSelection: true,
            // onColumnResized: function (event) { alert('a column was resized'); },
            onGridReady: function (event) {
                that.ClearRowSelection(event);
            },
            onGridSizeChanged: function (event) {
                console.log("gridSizeChanged");
                //event.api.sizeColumnsToFit();
                that.$scope.maingridOptions.api.sizeColumnsToFit();
            },
            // CALLBACKS
            processRowPostCreate: function (params) {
                $(params.eRow).attr("context-menu", "ctrl.menuOptions(data)");
                $(params.eRow).attr("model", "data");
            },
            onRowDoubleClicked: function (event) {
                //console.log("a row was double clicked ", event);
                //alert('a row was double clicked');                
                if (!event.data.folder)
                    return;
                that.$scope["OpenFolderByDoubleClick"] = true;
                that.OpenFolder(event.data.value);
                that.$scope.gridOptions.api.setFocusedCell();
            },
            onRowClicked: function (e) {
                e.event.preventDefault();
                that.$scope.gridOptions.api.setFocusedCell();
                that.$timeout(function () {
                    if (that.$scope.maingridOptions.rowSelection === 'single')
                        e.node.setSelected(true, true);
                    else
                        e.node.setSelected(true);
                });
            },
            overlayNoRowsTemplate: '<span style="">No item to display.</span>',
            onCellContextMenu: function (e) {
                e.event.preventDefault();
                if (that.$scope.maingridOptions.rowSelection === 'single')
                    e.node.setSelected(true, true);
                else
                    e.node.setSelected(true);
                //that.$scope.contextAttrs["model"] = e.data;
                //that.$scope.showContextmenu(e.event, that.$scope.contextAttrs);
            }
        };
    };
    FileControllerGrid.prototype.RefreshFileExplorer = function () {
        var that = this;
        this.$scope.gridOptions.api.showLoadingOverlay();
        //this.$scope.gridOptions.api.refreshView();
        //this.$scope.gridOptions.api.setRowData(null)
        this.$scope.gridOptions.api.setRowData(this.$scope.UserShares);
    };
    FileControllerGrid.prototype.RefreshMaingrid = function (arr) {
        this.$scope.maingridOptions.api.showLoadingOverlay();
        //this.$scope.maingridOptions.api.setRowData(null)
        this.$scope.maingridOptions.api.setRowData(arr);
        this.$scope.maingridOptions.api.sizeColumnsToFit();
        //this.$scope.maingridOptions.api.refreshView();
    };
    FileControllerGrid.prototype.GetFolderNode = function (key) {
        var that = this;
        var folderNode = null;
        that.$scope.gridOptions.api.forEachNode(function (node) {
            if (folderNode)
                return;
            var data = node.data;
            if (data.value === key) { // we found a group node!!!
                folderNode = node;
            }
        });
        return folderNode;
    };
    FileControllerGrid.prototype.GetFileNode = function (key) {
        var that = this;
        var fileNode = null;
        that.$scope.maingridOptions.api.forEachNode(function (node) {
            if (fileNode)
                return;
            var data = node.data;
            if (data.value === key) { // we found a group node!!!
                fileNode = node;
            }
        });
        return fileNode;
    };
    FileControllerGrid.prototype.OpenFolder = function (key) {
        var that = this;
        var folderNode = that.GetFolderNode(key);
        var node = folderNode;
        var path = node.data.name;
        if (!node.parent)
            node.data.open = node.expanded;
        while (node.parent) {
            node = node.parent;
            path = node.data.name + '\\' + path;
            node.data.open = node.expanded; // = true;
        }
        that.$scope["SelectedPath"] = path;
        if (folderNode["groupExpandclicked"] == undefined || folderNode["groupExpandclicked"] != true) {
            var key = folderNode.data.value;
            if (that.$scope["OpenFolderByDoubleClick"]) {
                folderNode.data.open = true;
                if (folderNode.parent) {
                    folderNode.parent.expanded = true;
                    folderNode.parent.data.open = true;
                }
                that.$scope["OpenFolderByDoubleClick"] = false;
            }
            that.RefreshGridsData(key, folderNode.data.open);
            return;
        }
        if (that.$scope.gridOptions.rowSelection === 'single')
            folderNode.setSelected(true, true);
        else
            folderNode.setSelected(true);
        folderNode["groupExpandclicked"] = false;
        that.SetGridFocusedRow();
    };
    FileControllerGrid.prototype.OpenParentFolder = function () {
        var that = this;
        var selectedRowsLeftGrid = that.$scope.gridOptions.api.getSelectedRows();
        if (!selectedRowsLeftGrid.length)
            return;
        var key = selectedRowsLeftGrid[0]["value"];
        var folderNode = that.GetFolderNode(key);
        if (!folderNode || !folderNode.parent)
            return;
        key = folderNode.parent.data.value;
        that.OpenFolder(key);
    };
    FileControllerGrid.prototype.RefreshGridsData = function (key, expanded) {
        var that = this;
        that.$scope.SelectedFolder = key;
        that.FileService.GetAllChildren({ id: key, filter: null })
            .then(function (resp) {
            var respData = [];
            angular.forEach(resp.data.Data, function (item, index) {
                respData.push(item);
            });
            var folderNodes = [];
            var allChildNodes = that.FileService.ConvertToMainGridModel(respData);
            angular.forEach(resp.data.Data, function (item, index) {
                if (item["FileEntryTypeId"] !== 2) //Only folder needs to list in Left 
                    return;
                folderNodes.push({
                    folder: true,
                    open: false,
                    name: item["FileVersion"]["FileEntryName"],
                    value: item["FileEntryId"],
                    children: [],
                    CanWrite: item["CanWrite"],
                    fileShareId: item["FileShareId"],
                    currentVersionNumber: item["CurrentVersionNumber"],
                    isDeleted: item["IsDeleted"] || item["FileVersion"]["IsDeleted"]
                });
            });
            var folderNode = that.GetFolderNode(key);
            //folderNode.data.open = expanded || folderNode.expanded;//makes group expanded
            folderNode.data.open = folderNode.expanded;
            folderNode.expanded = expanded;
            folderNode.data.children = folderNodes;
            that.RefreshFileExplorer();
            folderNode = that.GetFolderNode(key);
            folderNode.setSelected(true, true);
            that.$scope.maingridOptions.columnApi.setColumnVisible('path', false);
            that.RefreshMaingrid(allChildNodes);
            that.SetGridFocusedRow();
        }, function (resp) {
            //console.log(resp);
        });
    };
    FileControllerGrid.prototype.DownloadFile = function (selectedRow) {
        var that = this;
        //var selectedRow = this.$scope.maingridOptions.api.getSelectedRows()[0];
        //Get Azure download URL from Blob Name
        this.FileService.DownloadBlob(selectedRow.value, selectedRow.name, selectedRow.version)
            .then(function (resp) {
            //start file download
            if (resp.data.Data)
                window.location.href = resp.data.Data;
            else
                that.$scope.ShowAlert("Problem when downloading. Try after some time.", 2);
            //console.log(resp);                
        }, function (resp) {
            //console.log(resp);
        });
    };
    FileControllerGrid.prototype.DownloadDeletedFile = function (selectedRow) {
        var that = this;
        //Get Azure download URL from Blob Name
        this.FileService.DownloadDeletedBlob(selectedRow.value, selectedRow.name, selectedRow.version)
            .then(function (resp) {
            //start file download
            if (resp.data.Data)
                window.location.href = resp.data.Data;
            else
                that.$scope.ShowAlert("Problem when downloading. Try after some time.", 2);
        }, function (resp) {
            //console.log(resp);
        });
    };
    return FileControllerGrid;
}());
//all the modal dialogs like Account selection, Permission, Tags, File Upload, Create Folder, rename/move/delete file or folder
var FileControllerDialogs = /** @class */ (function (_super) {
    __extends(FileControllerDialogs, _super);
    function FileControllerDialogs(FileService, ngDialog, OrganizationService, PermissionService, TagService, $scope, $rootScope, $timeout, $compile, $filter, authInterceptorService) {
        var _this = _super.call(this, FileService, ngDialog, OrganizationService, PermissionService, TagService, $scope, $rootScope, $timeout, $compile, $filter, authInterceptorService) || this;
        _this.FileService = FileService;
        _this.ngDialog = ngDialog;
        _this.OrganizationService = OrganizationService;
        _this.PermissionService = PermissionService;
        _this.TagService = TagService;
        _this.$scope = $scope;
        _this.$rootScope = $rootScope;
        _this.$timeout = $timeout;
        _this.$compile = $compile;
        _this.$filter = $filter;
        _this.authInterceptorService = authInterceptorService;
        _this.OpenRenameDialog = function (fileEntryId, versionNumber, newName, folder) {
            var that = this;
            if (this.$scope.gridOptions.api.getSelectedRows().length <= 0)
                return;
            var dialog = this.ngDialog.openConfirm({
                template: 'modalRenameDialog',
                className: 'ngdialog-theme-default',
                controller: ['$scope', '$window', function ($scope, $window) {
                        // controller logic
                        var window = $window;
                        $scope.name = newName;
                        $scope.folderPattern = (function () {
                            var regexp = /^[a-z0-9_.@()-]+\.[^.][^.][^.]+$/i;
                            return {
                                test: function (value) {
                                    //if (folder === false) {
                                    //    return true;
                                    //}
                                    return regexp.test(value);
                                }
                            };
                        })();
                        //Create button click
                        $scope.RenameClick = function (e) {
                            // Rename folder
                            that.FileService.Rename(fileEntryId, versionNumber, $scope.name)
                                .then(function (resp) {
                                if (resp.data.Data === false) {
                                    that.$scope.ShowAlert(resp.data.Message, 2);
                                    return;
                                }
                                that.ReGenerateMainGridData(null);
                                $scope.closeThisDialog();
                                that.$scope.ShowAlert("Renamed successfully.", 1);
                                //console.log(resp);                           
                            }, function (resp) {
                                //console.log(resp);
                                that.$scope.ShowAlert("Error in rename -" + resp.data.Message, 2);
                            });
                        };
                    }],
            });
        };
        _this.OpenMoveDialog = function (model) {
            var that = this;
            var showAlert = that.$scope.ShowAlert;
            var innerCellRenderer = that.InnerCellRenderer;
            var fileService = that.FileService;
            var getChildren = function (folderId) {
                return fileService.GetAllChildren({ id: folderId, filter: null }).then(function (resp) {
                    if (resp && resp.data && resp.data.Data && resp.data.Data.length > 0) {
                        return [
                            resp.data.Data.filter(function (child) {
                                return child.FileEntryTypeId === 2;
                            }).map(function (child) {
                                return {
                                    fileShareId: child.FileShareId,
                                    value: child.FileEntryId,
                                    name: child.FileVersion.FileEntryNameWithExtension,
                                    CanWrite: child.CanWrite,
                                    children: [],
                                    open: false,
                                    folder: true
                                };
                            }), resp.data.Data.filter(function (child) {
                                return child.FileEntryTypeId === 3;
                            }).map(function (child) {
                                return {
                                    fileShareId: child.FileShareId,
                                    value: child.FileEntryId,
                                    name: child.FileVersion.FileEntryNameWithExtension,
                                    CanWrite: child.CanWrite
                                };
                            })
                        ];
                    }
                    return [[], []];
                });
            };
            var shares = angular.copy(that.$scope.UserShares.filter(function (userShare) {
                return userShare.fileShareId === model.fileShareId;
            }));
            if (shares && shares.length > 0) {
                shares = [shares[0]];
                shares[0].open = true;
                getChildren(shares[0].value).then(function (folderChildren) {
                    shares[0].children = folderChildren[0];
                    shares[0].childrenFiles = folderChildren[1];
                    that.ngDialog.openConfirm({
                        template: "modalMoveDialog",
                        className: "ngdialog-theme-default wider-dialog",
                        controller: ["$scope", function ($scope) {
                                $scope.folderOrFileName = model.name;
                                $scope.selectedChildFolder = null;
                                $scope.moveOptions = {
                                    columnDefs: [
                                        {
                                            headerName: "Select folder to move file into", field: "name",
                                            cellRenderer: 'group',
                                            cellRendererParams: {
                                                innerRenderer: innerCellRenderer
                                            }
                                        }
                                    ],
                                    rowData: shares,
                                    rowSelection: 'single',
                                    enableColResize: false,
                                    enableSorting: false,
                                    angularCompileRows: true,
                                    rowHeight: 20,
                                    getNodeChildDetails: function (file) {
                                        if (file.folder) {
                                            return {
                                                group: true,
                                                children: file.children,
                                                expanded: file.open
                                            };
                                        }
                                        else {
                                            return null;
                                        }
                                    },
                                    icons: {
                                        groupExpanded: '<i class="fa fa-minus-square-o"/>',
                                        groupContracted: '<i class="fa fa-plus-square-o"/>'
                                    },
                                    onGridReady: function (event) {
                                        event.api.sizeColumnsToFit();
                                    },
                                    onRowClicked: function (event) {
                                        var selectedChildFolder = event.data;
                                        if (event.event.target.tagName.toLowerCase() === "i") {
                                            selectedChildFolder.open = event.node.expanded;
                                            if (event.node.expanded) {
                                                getChildren(selectedChildFolder.value).then(function (folderChildren) {
                                                    selectedChildFolder.children = folderChildren[0];
                                                    selectedChildFolder.childrenFiles = folderChildren[1];
                                                    event.api.setRowData(shares);
                                                    event.api.refreshView();
                                                }, function (reason) {
                                                    //console.log(reason);
                                                    selectedChildFolder.childrenFiles = [];
                                                });
                                            }
                                            else {
                                                selectedChildFolder.children = [];
                                                selectedChildFolder.childrenFiles = [];
                                                event.api.setRowData(shares);
                                                event.api.refreshView();
                                            }
                                        }
                                        setTimeout(function () {
                                            $scope.$apply(function () {
                                                $scope.selectedChildFolder = selectedChildFolder;
                                            });
                                        });
                                    },
                                    overlayNoRowsTemplate: '<span style="">No Shares.</span>'
                                };
                                $scope.Move = function () {
                                    var selectedChildFolder = $scope.selectedChildFolder;
                                    var selectedFolderId = that.$scope.gridOptions.api.getSelectedRows()[0].value;
                                    if (selectedFolderId === selectedChildFolder.value) {
                                        that.$scope.ShowAlert("Source and target folder are same. Can not move selected file!", 2);
                                        return;
                                    }
                                    var moveDecision = function () {
                                        that.ngDialog.openConfirm({
                                            template: "modalMoveDecisionDialog",
                                            className: "ngdialog-theme-default",
                                            controller: ["$scope", function ($scope) {
                                                    $scope.selectedChildFolder = selectedChildFolder;
                                                    $scope.selectedFile = model;
                                                    $scope.Replace = function () {
                                                        $scope.closeThisDialog();
                                                        moveFile(true);
                                                    };
                                                    $scope.Rename = function () {
                                                        $scope.closeThisDialog();
                                                        that.ngDialog.openConfirm({
                                                            template: 'modalMoveDecisionRenameDialog',
                                                            className: 'ngdialog-theme-default',
                                                            controller: ['$scope', function ($scope) {
                                                                    $scope.name = model.name;
                                                                    $scope.pattern = (function () {
                                                                        return {
                                                                            test: function (value) {
                                                                                return /^[a-z0-9_.@()-]+\.[^.][^.][^.]+$/i.test(value);
                                                                            }
                                                                        };
                                                                    })();
                                                                    $scope.RenameAndMove = function () {
                                                                        $scope.closeThisDialog();
                                                                        moveFile(false, $scope.name);
                                                                    };
                                                                }],
                                                        });
                                                    };
                                                }]
                                        });
                                    };
                                    var moveFile = function (isReplaceExistingFile, newFileName) {
                                        if (isReplaceExistingFile === void 0) { isReplaceExistingFile = false; }
                                        if (newFileName === void 0) { newFileName = ""; }
                                        fileService.Move(model.value, selectedChildFolder.value, isReplaceExistingFile, newFileName).then(function (resp) {
                                            if (resp && resp.data) {
                                                if (resp.data.Data) {
                                                    $scope.closeThisDialog();
                                                    showAlert("File moved.", 1);
                                                    that.ReGenerateMainGridData(null);
                                                    return;
                                                }
                                                else if (resp.data.Status === 2) {
                                                    moveDecision();
                                                    return;
                                                }
                                                else if (resp.data.Message) {
                                                    showAlert("File could not be moved due to following reason:<ul><li>" + resp.data.Message + "</li></ul>", 2);
                                                    return;
                                                }
                                            }
                                            showAlert("File could not be moved.", 2);
                                        }, function (resp) {
                                            //console.log(resp);
                                        });
                                    };
                                    if ($scope.selectedChildFolder.childrenFiles && $scope.selectedChildFolder.childrenFiles.length > 0 && $scope.selectedChildFolder.childrenFiles.filter(function (childrenFile) { return childrenFile.name === model.name; }).length > 0) {
                                        moveDecision();
                                    }
                                    else {
                                        moveFile();
                                    }
                                };
                            }]
                    });
                }, function (reason) {
                    //console.log(reason);
                    shares[0].childrenFiles = [];
                });
            }
            else {
                showAlert("Unable to determine share of the selected file.", 2);
            }
        };
        _this.DeleteRestorePurgeDialog = function (model, action, selectedNode) {
            if (selectedNode === void 0) { selectedNode = null; }
            var that = this;
            var selectedFolderId = that.$scope.gridOptions.api.getSelectedRows()[0].value;
            var fileList = [];
            for (var i = 0; i < that.$scope.maingridOptions.api.getSelectedRows().length; i++) {
                var file = that.$scope.maingridOptions.api.getSelectedRows()[i];
                fileList.push({
                    fileEntryId: file.value,
                    localFileVersionNumber: file.currentVersionNumber
                });
            }
            if (selectedNode) {
                var folderNode = this.GetFolderNode(selectedNode.value);
                fileList = [];
                selectedFolderId = folderNode.parent.data.value;
                fileList.push({
                    fileEntryId: model.value,
                    localFileVersionNumber: model.currentVersionNumber
                });
            }
            var showAlert = that.$scope.ShowAlert;
            var fileService = that.FileService;
            that.ngDialog.openConfirm({
                template: "modalDeleteRestorePurgeDialog",
                className: "ngdialog-theme-default",
                controller: ["$scope", function ($scope) {
                        $scope.action = action;
                        $scope.isAdmin = that.$rootScope.userDetail && that.$rootScope.userDetail.IsAdmin === true;
                        $scope.isAccountAdmin = that.$rootScope.IsAccountAdmin;
                        $scope.folderOrFileName = fileList.length > 1 ? "All Items" : model.name;
                        $scope.isFolder = model.folder;
                        $scope.Action = function (action) {
                            var actionMethod = null;
                            switch (action) {
                                case "Delete":
                                    action = "deleted";
                                    actionMethod = fileService.DeleteMultiple; //.Delete;
                                    break;
                                case "Restore":
                                    action = "restored";
                                    actionMethod = fileService.Restore;
                                    break;
                                case "Purge":
                                    action = "purged";
                                    actionMethod = fileService.PurgeMultiple; //.Purge;
                                    break;
                            }
                            if (actionMethod) {
                                actionMethod.call(fileService, fileList).then(function (resp) {
                                    if (resp && resp.data) {
                                        if (resp.data.Data) {
                                            $scope.closeThisDialog();
                                            showAlert("Folder/File " + action + ".", 1);
                                            //that.ReGenerateMainGridData(null);
                                            //that.RefreshGridsData(selectedFolderId, false);
                                            that.OpenFolder(selectedFolderId);
                                            return;
                                        }
                                        else if (resp.data.Message) {
                                            $scope.closeThisDialog();
                                            showAlert("Folder/File could not be " + action + " due to following reason:<ul><li>" + resp.data.Message + "</li></ul>", 2);
                                            return;
                                        }
                                    }
                                    showAlert("Folder/File could not be " + action + ".", 2);
                                }, function (resp) {
                                    //console.log(resp);
                                    $scope.closeThisDialog();
                                });
                            }
                            else {
                                $scope.closeThisDialog();
                            }
                        };
                    }]
            });
        };
        _this.OpenConfirmDialog = function (bodyText, titleText, callback) {
            var that = this;
            var confirmDialog = that.ngDialog.openConfirm({
                template: 'modalConfirmDialog',
                className: 'ngdialog-theme-default',
                width: 400,
                controller: ['$scope', '$window', function ($scope, $window) {
                        //console.log(confirmDialog);
                        if (titleText)
                            $scope.confirmHeader = titleText;
                        $scope.bodyText = bodyText || "Are you sure?";
                        $scope.confirm = function (data) {
                            if (data && !data.value)
                                return;
                            if (callback)
                                callback.apply();
                            $scope.closeThisDialog();
                        };
                    }],
            });
        };
        _this.OpenDeletedFileDialog = function () {
            var that = this;
            if (this.$scope.gridOptions.api.getSelectedRows().length <= 0)
                return;
            var selectedFolderId = this.$scope.gridOptions.api.getSelectedRows()[0].value;
            var dialog = this.ngDialog.openConfirm({
                template: 'modalDeletedFilesDialog',
                className: 'ngdialog-theme-default deleted-files-modal',
                controller: ['$scope', '$window', function ($scope, $window) {
                        // controller logic
                        var window = $window;
                        var deletedFileColumnDefs = [
                            {
                                headerName: "Name", field: "name", width: 250,
                                cellRenderer: function (params) {
                                    var image;
                                    if (params.data.folder) {
                                        image = 'folder';
                                    }
                                    else {
                                        image = 'file';
                                    }
                                    var imageFullUrl = '/Images/' + image + '.png';
                                    var value = '<img src="' + imageFullUrl + '" style="padding-left: 4px;" /> ' + params.data.name;
                                    return value;
                                }
                            },
                            { headerName: "Path", field: "path", width: 250 },
                            //{ headerName: "Version", field: "version", width: 75, cellStyle: sizeCellStyle },
                            { headerName: "Deleted By", field: "DeletedBy", width: 250 },
                            { headerName: "Deleted On", field: "DeletedOn", width: 150 },
                            {
                                headerName: "Action", field: "", width: 150,
                                cellRenderer: function (params) {
                                    var buttonStr = '<button id="btn" class="btn btn-danger btn-xs" title= "Permanent Delete" ' + " ng-click='DeleteFileGridActionClick(\"PermanentDelete\", $event, \"" + params.data.value + "\")' >" +
                                        '<i class="glyphicon glyphicon-trash" > </i>' +
                                        '</button>' +
                                        '<button id= "btn" class="btn btn-success margin-left-15 btn-xs" title= "Restore" ' + " ng-click='DeleteFileGridActionClick(\"Restore\", $event,  \"" + params.data.value + "\")' >" +
                                        '<i class="glyphicon glyphicon-repeat" > </i>' +
                                        '</button>';
                                    if (!params.data.folder) {
                                        buttonStr +=
                                            '<button id= "btn" class="btn btn-primary margin-left-15 btn-xs"  title= "Download file" ' + " ng-click='DeleteFileGridActionClick(\"Download\", $event,  \"" + params.data.value + "\")' >" +
                                                '<i class="glyphicon glyphicon-cloud-download" > </i>' +
                                                '</button>';
                                    }
                                    return buttonStr;
                                }
                            }
                        ];
                        $scope.deletedFileOptions = {
                            // PROPERTIES 
                            columnDefs: deletedFileColumnDefs,
                            rowData: null,
                            rowSelection: 'single',
                            enableColResize: true,
                            enableSorting: true,
                            rowHeight: 35,
                            suppressRowClickSelection: false,
                            angularCompileRows: true,
                            suppressLoadingOverlay: true,
                            suppressCellSelection: true,
                            onGridReady: function (event) {
                                that.ClearRowSelection(event);
                            },
                            overlayNoRowsTemplate: '<span style="">No item to display.</span>',
                        };
                        var refreshContent = function () {
                            //key = folderNode.data.value;
                            that.FileService.GetAllDeletedChildren({ id: selectedFolderId, filter: null })
                                .then(function (resp) {
                                if (resp.data.Status == 404) {
                                    //folderNode = folderNode.parent;
                                    //if (folderNode != null)
                                    //    refreshContent();
                                    return;
                                }
                                var respData = [];
                                angular.forEach(resp.data.Data, function (item, index) {
                                    respData.push(item);
                                });
                                var deletedFileNodes = [];
                                //var allChildNodes = that.FileService.ConvertToDeletedFileModel(respData)
                                angular.forEach(resp.data.Data, function (item, index) {
                                    if (item["FileEntryTypeId"] === 1 && !(item["IsDeleted"] || item["FileVersion"]["IsDeleted"])) //Only deleted files needs to list 
                                        return;
                                    deletedFileNodes.push({
                                        folder: item["FileEntryTypeId"] === 2,
                                        name: item["FileVersion"]["FileEntryName"],
                                        value: item["FileEntryId"],
                                        path: item["FileVersion"]["FileEntryRelativePath"],
                                        children: [],
                                        CanWrite: item["CanWrite"],
                                        fileShareId: item["FileShareId"],
                                        version: item["CurrentVersionNumber"],
                                        DeletedBy: item["DeletedByUserName"],
                                        DeletedOn: new Date(item["DeletedOnUTC"]).toLocaleString("en-IN"),
                                        isDeleted: item["IsDeleted"] || item["FileVersion"]["IsDeleted"]
                                    });
                                });
                                //$scope.deletedFileOptions.columnApi.setColumnVisible('path', false);
                                $scope.deletedFileOptions.api.showLoadingOverlay();
                                $scope.deletedFileOptions.api.setRowData(deletedFileNodes);
                                $scope.deletedFileOptions.api.sizeColumnsToFit();
                            }, function (resp) {
                                //console.log(resp);
                            });
                        };
                        refreshContent();
                        //button click
                        $scope.DeleteFileGridActionClick = function (action, e, fileId) {
                            //console.log(action, e, fileId);
                            if (action === "PermanentDelete") {
                                that.OpenConfirmDialog("Are you sure you want to permanently delete this file?", null, function () {
                                    that.FileService.Purge(fileId)
                                        .then(function (resp) {
                                        if (resp.data.Status == 404) {
                                            return;
                                        }
                                        if (resp.data.Data == false) {
                                            that.$scope.ShowAlert(resp.data.Message, 2);
                                            return;
                                        }
                                        refreshContent();
                                    });
                                });
                            }
                            else if (action === "Restore") {
                                that.FileService.Restore(fileId)
                                    .then(function (resp) {
                                    if (resp.data.Status == 404)
                                        return;
                                    if (resp.data.Data == false) {
                                        that.$scope.ShowAlert(resp.data.Message, 2);
                                        return;
                                    }
                                    that.$scope.ShowAlert(resp.data.Message, 1);
                                    refreshContent();
                                });
                            }
                            else if (action === "Download") {
                                $scope.deletedFileOptions.api.forEachNode(function (node, index) {
                                    //make grid row selected when click button
                                    if (node.data.value === fileId) {
                                        node.setSelected(true, true);
                                    }
                                });
                                var selectedRow = $scope.deletedFileOptions.api.getSelectedRows()[0];
                                if (selectedRow)
                                    that.DownloadDeletedFile(selectedRow);
                            }
                            else if (action === "closeDialog") {
                                //that.ReGenerateMainGridData(null);
                                that.RefreshGridsData(selectedFolderId, false);
                                $scope.closeThisDialog();
                            }
                        };
                    }],
            });
        };
        return _this;
    }
    FileControllerDialogs.prototype.openPermissionDialog = function (model) {
        var _this = this;
        var that = this;
        if (!model)
            model = this.$scope.gridOptions.api.getSelectedRows()[0];
        var permissionService = this.PermissionService;
        var organizationService = this.OrganizationService;
        var filterService = this.$filter;
        var showAlert = this.$scope.ShowAlert;
        permissionService.getEditPermission(model.value).then(function (resp) {
            if (resp && resp.data) {
                var isEditable = resp.data.Data["CanEdit"];
                var isCanChangeShareAdminPermission = resp.data.Data["CanShare"];
                _this.ngDialog.openConfirm({
                    template: "modalPermissionDialogId",
                    className: "ngdialog-theme-default wider-dialog",
                    controller: ["$scope", function ($scope) {
                            $scope.folderOrFileName = model.name;
                            $scope.isEditable = isEditable;
                            $scope.isCanChangeShareAdminPermission = isCanChangeShareAdminPermission;
                            $scope.usersAndGroups = [];
                            $scope.selectedFile = model.value;
                            $scope.emptyUserOrGroupPermissionsForFileAndFolder = {
                                userOrGroup: null,
                                permissions: [
                                    {
                                        Name: "Read",
                                        Value: 1,
                                        isAllowed: false,
                                        isAllowedDisabled: false,
                                        isDenied: false,
                                        isDeniedDisabled: false
                                    }, {
                                        Name: "Write",
                                        Value: 2,
                                        isAllowed: false,
                                        isAllowedDisabled: false,
                                        isDenied: false,
                                        isDeniedDisabled: false
                                    }, {
                                        Name: "Edit Tag",
                                        Value: 4,
                                        isAllowed: false,
                                        isAllowedDisabled: false,
                                        isDenied: false,
                                        isDeniedDisabled: false
                                        //}, {
                                        //    Name: "List",
                                        //    Value: 8,
                                        //    isAllowed: false,
                                        //    isAllowedDisabled: false,
                                        //    isDenied: false,
                                        //    isDeniedDisabled: false
                                    }
                                ]
                            };
                            $scope.emptyUserOrGroupPermissionsForShare = angular.copy($scope.emptyUserOrGroupPermissionsForFileAndFolder);
                            $scope.emptyUserOrGroupPermissionsForShare.permissions.push({
                                Name: "Share Admin",
                                Value: 16,
                                isAllowed: false,
                                isAllowedDisabled: false,
                                isDenied: false,
                                isDeniedDisabled: true
                            });
                            $scope.usersAndGroupsPermissions = {};
                            $scope.removedUsersAndGroupsPermissions = [];
                            $scope.userOrGroupPermissions = null;
                            $scope.usersAndGroupsOptions = {
                                angularCompileRows: true,
                                columnDefs: [
                                    {
                                        headerName: "Users or Groups",
                                        cellRenderer: function (params) {
                                            var userOrGroup = params.data;
                                            return "<i userorgroup='" + JSON.stringify({ Id: userOrGroup.Id, Type: userOrGroup.Type }) + "' class='fa fa-" + (userOrGroup.Type === 1 ? "user" : (userOrGroup.Type === 2 ? "users" : "user-secret")) + "'></i> " + userOrGroup.Name;
                                        }
                                    },
                                    {
                                        headerName: "",
                                        cellRenderer: function (params) {
                                            return ($scope.isEditable ? "<button class='btn btn-xs btn-danger' ng-click='RemoveUserOrGroupRow(data, $event)'><i class='fa fa-minus' title='Remove row'></i></button>" : "");
                                        },
                                        width: 30,
                                        cellStyle: { 'text-align': 'center' }
                                    }
                                ],
                                rowData: $scope.usersAndGroups,
                                enableColResize: false,
                                enableSorting: false,
                                rowHeight: 30,
                                onGridReady: function (event) {
                                    event.api.sizeColumnsToFit();
                                },
                                onRowClicked: function (event) {
                                    var loweredEventTargetTagName = event.event.target.tagName.toLowerCase();
                                    if (loweredEventTargetTagName !== "button" && loweredEventTargetTagName !== "i") {
                                        $scope.userOrGroupPermissions = null;
                                        var userOrGroup = event.data;
                                        var setPermissions = function (resp, isExisting) {
                                            if (isExisting === void 0) { isExisting = false; }
                                            //console.log(resp);
                                            if (isExisting) {
                                                $scope.userOrGroupPermissions = $scope.usersAndGroupsPermissions[JSON.stringify({ Id: userOrGroup.Id, Type: userOrGroup.Type })].userOrGroupPermissions;
                                            }
                                            else {
                                                var permissions = resp.data.Data;
                                                var isAnyInheritedPermissionFound = false;
                                                var effectivePermissionsSources = [];
                                                if (permissions.EffectivePermissionsSources) {
                                                    try {
                                                        var EffectivePermissionsSources = JSON.parse(permissions.EffectivePermissionsSources);
                                                        if (EffectivePermissionsSources && EffectivePermissionsSources.length > 0) {
                                                            effectivePermissionsSources = EffectivePermissionsSources;
                                                        }
                                                    }
                                                    catch (e) { }
                                                }
                                                $scope.userOrGroupPermissions = angular.copy(model.isShare && $scope.isCanChangeShareAdminPermission ? $scope.emptyUserOrGroupPermissionsForShare : $scope.emptyUserOrGroupPermissionsForFileAndFolder);
                                                $scope.userOrGroupPermissions.userOrGroup = userOrGroup;
                                                angular.forEach($scope.userOrGroupPermissions.permissions, function (permission) {
                                                    var effectivePermissionsSource = effectivePermissionsSources.find(function (effectivePermissionsSource) { return effectivePermissionsSource.Permission == permission.Value; });
                                                    if (effectivePermissionsSource && (effectivePermissionsSource.AllowedPath || effectivePermissionsSource.DeniedPath)) {
                                                        angular.extend(permission, { InheritedFrom: (effectivePermissionsSource.DeniedPath ? effectivePermissionsSource.DeniedPath : effectivePermissionsSource.AllowedPath) });
                                                        isAnyInheritedPermissionFound = true;
                                                    }
                                                    if (permission.Value & permissions.EffectiveDeny) {
                                                        angular.extend(permission, { isAllowedDisabled: true, isDenied: true, isDeniedDisabled: true });
                                                    }
                                                    else {
                                                        if (permission.Value & permissions.EffectiveAllow) {
                                                            angular.extend(permission, { isAllowed: true, isAllowedDisabled: true });
                                                        }
                                                        if (permission.Value & permissions.ExclusiveAllow) {
                                                            angular.extend(permission, { isAllowed: true });
                                                        }
                                                        if (permission.Value & permissions.ExclusiveDeny) {
                                                            angular.extend(permission, { isDenied: true });
                                                        }
                                                    }
                                                });
                                                $scope.userOrGroupPermissions.isAnyInheritedPermissionFound = isAnyInheritedPermissionFound;
                                                $scope.usersAndGroupsPermissions[JSON.stringify({ Id: userOrGroup.Id, Type: userOrGroup.Type })] = {
                                                    userOrGroupPermissions: $scope.userOrGroupPermissions,
                                                    originalUserOrGroupPermissions: angular.copy($scope.userOrGroupPermissions)
                                                };
                                            }
                                            $scope.userOrGroupPermissionsOptions.columnDefs[0].headerName = "Permissions for: " + userOrGroup.Name;
                                            if (!$scope.userOrGroupPermissions.isAnyInheritedPermissionFound) {
                                                $scope.userOrGroupPermissionsOptions.columnDefs[3].hide = true;
                                            }
                                            $scope.userOrGroupPermissionsOptions.api.setColumnDefs($scope.userOrGroupPermissionsOptions.columnDefs);
                                            $scope.userOrGroupPermissionsOptions.api.setRowData($scope.userOrGroupPermissions.permissions);
                                            $scope.userOrGroupPermissionsOptions.api.refreshView();
                                            $scope.userOrGroupPermissionsOptions.api.sizeColumnsToFit();
                                        };
                                        var userOrGroupPermissions = $scope.usersAndGroupsPermissions[JSON.stringify({ Id: userOrGroup.Id, Type: userOrGroup.Type })];
                                        if (userOrGroupPermissions && userOrGroupPermissions.userOrGroupPermissions && userOrGroupPermissions.userOrGroupPermissions.userOrGroup && userOrGroupPermissions.userOrGroupPermissions.permissions) {
                                            setPermissions(null, true);
                                        }
                                        else {
                                            switch (userOrGroup.Type) {
                                                case 1:
                                                    permissionService.getPermissionByFileAndUser(model.value, userOrGroup.Id).then(setPermissions, function (resp) {
                                                        console.log(resp);
                                                    });
                                                    break;
                                                case 2:
                                                    permissionService.getPermissionByFileAndGroup(model.value, userOrGroup.Id).then(setPermissions, function (resp) {
                                                        console.log(resp);
                                                    });
                                                    break;
                                                default:
                                            }
                                        }
                                    }
                                },
                                overlayNoRowsTemplate: "<span>No User or Group found.</span>"
                            };
                            $scope.addUserOrGroupRow = function (newlyAddedUserOrGroup) {
                                $scope.userOrGroupPermissions = null;
                                var existingUserOrGroup = filterService('filter')($scope.usersAndGroups, { Id: newlyAddedUserOrGroup.Id, Type: newlyAddedUserOrGroup.Type }, true)[0];
                                if (!existingUserOrGroup) {
                                    existingUserOrGroup = filterService('filter')($scope.removedUsersAndGroupsPermissions, { userOrGroupPermissions: { userOrGroup: { Id: newlyAddedUserOrGroup.Id, Type: newlyAddedUserOrGroup.Type } } }, true)[0];
                                    if (existingUserOrGroup) {
                                        $scope.removedUsersAndGroupsPermissions.splice($scope.removedUsersAndGroupsPermissions.indexOf(existingUserOrGroup), 1);
                                        existingUserOrGroup.originalUserOrGroupPermissions.userOrGroup.Name = newlyAddedUserOrGroup.Name;
                                        var userOrGroup = existingUserOrGroup.userOrGroupPermissions.userOrGroup;
                                        $scope.usersAndGroupsPermissions[JSON.stringify({ Id: userOrGroup.Id, Type: userOrGroup.Type })] = existingUserOrGroup;
                                        existingUserOrGroup = userOrGroup;
                                        $scope.usersAndGroups.push(existingUserOrGroup);
                                    }
                                }
                                if (existingUserOrGroup) {
                                    existingUserOrGroup.Name = newlyAddedUserOrGroup.Name;
                                }
                                else {
                                    newlyAddedUserOrGroup.isNewlyAddedUserOrGroup = true;
                                    $scope.usersAndGroups.push(newlyAddedUserOrGroup);
                                    existingUserOrGroup = newlyAddedUserOrGroup;
                                }
                                $scope.usersAndGroupsOptions.api.setRowData($scope.usersAndGroups);
                                $scope.usersAndGroupsOptions.api.refreshView();
                                $scope.usersAndGroupsOptions.api.setFocusedCell($scope.usersAndGroups.indexOf(existingUserOrGroup));
                                $($scope.usersAndGroupsOptions.api.rowRenderer.eBodyContainer).find("i[userorgroup='" + JSON.stringify({ Id: existingUserOrGroup.Id, Type: existingUserOrGroup.Type }) + "']").closest(".ag-row").click();
                            };
                            $scope.AddUserOrGroupRow = function () {
                                var addUserOrGroupRow = $scope.addUserOrGroupRow;
                                that.ngDialog.openConfirm({
                                    template: "modalUserOrGroupSelectionDialogId",
                                    className: "ngdialog-theme-default",
                                    controller: ["$scope", function ($scope) {
                                            $scope.userOrGroupName = "";
                                            $scope.usersAndGroups = [];
                                            $scope.selectedUserOrGroup = null;
                                            $scope.usersAndGroupsOptions = {
                                                angularCompileRows: true,
                                                columnDefs: [{
                                                        headerName: "Users and Groups",
                                                        cellRenderer: function (params) {
                                                            var userOrGroup = params.data;
                                                            return "<i class='fa fa-" + (userOrGroup.Type === 1 ? "user" : (userOrGroup.Type === 2 ? "users" : "user-secret")) + "'></i> " + userOrGroup.Name;
                                                        }
                                                    }],
                                                rowData: $scope.usersAndGroups,
                                                enableColResize: false,
                                                enableSorting: false,
                                                rowHeight: 25,
                                                overlayNoRowsTemplate: "<span>No User or Group found.</span>",
                                                rowSelection: 'single',
                                                onSelectionChanged: function () {
                                                    setTimeout(function () {
                                                        $scope.$apply(function () {
                                                            var selectedUsersOrGroups = $scope.usersAndGroupsOptions.api.getSelectedRows();
                                                            if (selectedUsersOrGroups && selectedUsersOrGroups.length > 0) {
                                                                $scope.selectedUserOrGroup = selectedUsersOrGroups[0];
                                                            }
                                                            else {
                                                                $scope.selectedUserOrGroup = null;
                                                            }
                                                        });
                                                    });
                                                }
                                            };
                                            $scope.CheckNames = function () {
                                                $scope.selectedUserOrGroup = null;
                                                $scope.usersAndGroups = [];
                                                organizationService.GetAccountGroupUsers(that.$rootScope.selectedAccount.accountId, $scope.userOrGroupName).then(function (resp) {
                                                    //console.log(resp);
                                                    $scope.usersAndGroups = resp.data.Data;
                                                    $scope.usersAndGroupsOptions.api.setRowData($scope.usersAndGroups);
                                                    $scope.usersAndGroupsOptions.api.refreshView();
                                                    $scope.usersAndGroupsOptions.api.deselectAll();
                                                    $scope.usersAndGroupsOptions.api.setFocusedCell(-1);
                                                    $scope.usersAndGroupsOptions.api.sizeColumnsToFit();
                                                }, function (resp) {
                                                    console.log(resp);
                                                });
                                            };
                                            $scope.Save = function () {
                                                if ($scope.selectedUserOrGroup) {
                                                    addUserOrGroupRow($scope.selectedUserOrGroup);
                                                    $scope.closeThisDialog();
                                                }
                                            };
                                        }]
                                });
                            };
                            $scope.RemoveUserOrGroupRow = function (userOrGroup, event) {
                                if ($scope.usersAndGroups.length > 0) {
                                    var index = $scope.usersAndGroups.indexOf(userOrGroup);
                                    if (index >= 0) {
                                        var agBodyContainer = $(event.target).closest(".ag-body-container");
                                        $scope.usersAndGroups.splice(index, 1);
                                        if (!userOrGroup.isNewlyAddedUserOrGroup) {
                                            var userOrGroupPermissions = $scope.usersAndGroupsPermissions[JSON.stringify({ Id: userOrGroup.Id, Type: userOrGroup.Type })];
                                            if (userOrGroupPermissions) {
                                                $scope.removedUsersAndGroupsPermissions.push(userOrGroupPermissions);
                                            }
                                            else {
                                                userOrGroupPermissions = {
                                                    userOrGroup: userOrGroup
                                                };
                                                $scope.removedUsersAndGroupsPermissions.push({
                                                    userOrGroupPermissions: userOrGroupPermissions,
                                                    originalUserOrGroupPermissions: angular.copy(userOrGroupPermissions)
                                                });
                                            }
                                        }
                                        delete $scope.usersAndGroupsPermissions[JSON.stringify({ Id: userOrGroup.Id, Type: userOrGroup.Type })];
                                        $scope.userOrGroupPermissions = null;
                                        $scope.usersAndGroupsOptions.api.setRowData($scope.usersAndGroups);
                                        $scope.usersAndGroupsOptions.api.refreshView();
                                        agBodyContainer.find(".ag-row.ag-row-focus").click();
                                    }
                                }
                            };
                            $scope.Save = function () {
                                var addedUsersAndGroupsPermissions = [];
                                var updatedUsersAndGroupsPermissions = [];
                                var removedUsersAndGroupsPermissions = $scope.removedUsersAndGroupsPermissions.map(function (removedUserOrGroupPermission) {
                                    var userOrGroup = removedUserOrGroupPermission.userOrGroupPermissions.userOrGroup;
                                    return {
                                        Id: userOrGroup.Id,
                                        Type: userOrGroup.Type
                                    };
                                });
                                var getPermissions = function (userOrGroupPermissions) {
                                    var permissions = {
                                        Id: userOrGroupPermissions.userOrGroup.Id,
                                        Type: userOrGroupPermissions.userOrGroup.Type,
                                        EffectiveAllow: 0,
                                        EffectiveDeny: 0,
                                        ExclusiveAllow: 0,
                                        ExclusiveDeny: 0
                                    };
                                    angular.forEach(userOrGroupPermissions.permissions, function (permission) {
                                        if (permission.isAllowed || permission.isDenied) {
                                            if (permission.isAllowed) {
                                                if (permission.isAllowedDisabled) {
                                                    permissions.EffectiveAllow |= permission.Value;
                                                }
                                                else {
                                                    permissions.ExclusiveAllow |= permission.Value;
                                                }
                                            }
                                            if (permission.isDenied) {
                                                if (permission.isDeniedDisabled) {
                                                    permissions.EffectiveDeny |= permission.Value;
                                                }
                                                else {
                                                    permissions.ExclusiveDeny |= permission.Value;
                                                }
                                            }
                                        }
                                    });
                                    return permissions;
                                };
                                angular.forEach($scope.usersAndGroupsPermissions, function (userOrGroupPermissions) {
                                    if (!angular.equals(userOrGroupPermissions.originalUserOrGroupPermissions, userOrGroupPermissions.userOrGroupPermissions)) {
                                        if (userOrGroupPermissions.userOrGroupPermissions.userOrGroup.isNewlyAddedUserOrGroup) {
                                            addedUsersAndGroupsPermissions.push(getPermissions(userOrGroupPermissions.userOrGroupPermissions));
                                        }
                                        else {
                                            updatedUsersAndGroupsPermissions.push(getPermissions(userOrGroupPermissions.userOrGroupPermissions));
                                        }
                                    }
                                });
                                permissionService.savePermissionByFileAndGroup($scope.selectedFile, { AddedUsersAndGroupsPermissions: addedUsersAndGroupsPermissions, UpdatedUsersAndGroupsPermissions: updatedUsersAndGroupsPermissions, RemovedUsersAndGroupsPermissions: removedUsersAndGroupsPermissions }).then(function (resp) {
                                    if (resp && resp.data) {
                                        if (resp.data.Data) {
                                            $scope.closeThisDialog();
                                            showAlert("Permissions assigned/changed.", 1);
                                            return;
                                        }
                                        else if (resp.data.Message) {
                                            showAlert("Permissions could not be assigned/changed due to following reason:<ul><li>" + resp.data.Message + "</li></ul>", 2);
                                            return;
                                        }
                                    }
                                    showAlert("Permissions could not be assigned/changed.", 2);
                                }, function (resp) {
                                    //console.log(resp);
                                });
                            };
                            $scope.HandlePermissionChange = function (permission, property, event) {
                                var permissions = $scope.userOrGroupPermissions.permissions;
                                var permissionValue = event.target.checked;
                                var propertyToValidate = (property === "isAllowed" ? "isDenied" : "isAllowed");
                                var enableAllAllow = function () {
                                    for (var index = 0; index < permissions.indexOf(permission); index++) {
                                        if (!permissions[index].isAllowedDisabled) {
                                            permissions[index].isAllowed = true;
                                        }
                                    }
                                    $scope.userOrGroupPermissionsOptions.api.setRowData($scope.userOrGroupPermissions.permissions);
                                    $scope.userOrGroupPermissionsOptions.api.refreshView();
                                };
                                var disableAllAllow = function () {
                                    for (var index = permissions.length - 1; index > permissions.indexOf(permission); index--) {
                                        if (!permissions[index].isAllowedDisabled) {
                                            permissions[index].isAllowed = false;
                                        }
                                    }
                                    $scope.userOrGroupPermissionsOptions.api.setRowData($scope.userOrGroupPermissions.permissions);
                                    $scope.userOrGroupPermissionsOptions.api.refreshView();
                                };
                                var enableAllDeny = function () {
                                    for (var index = permissions.length - 1; index > permissions.indexOf(permission); index--) {
                                        if (!permissions[index].isDeniedDisabled) {
                                            permissions[index].isDenied = true;
                                        }
                                    }
                                    $scope.userOrGroupPermissionsOptions.api.setRowData($scope.userOrGroupPermissions.permissions);
                                    $scope.userOrGroupPermissionsOptions.api.refreshView();
                                };
                                var disableAllDeny = function () {
                                    for (var index = 0; index < permissions.indexOf(permission); index++) {
                                        if (!permissions[index].isDeniedDisabled) {
                                            permissions[index].isDenied = false;
                                        }
                                    }
                                    $scope.userOrGroupPermissionsOptions.api.setRowData($scope.userOrGroupPermissions.permissions);
                                    $scope.userOrGroupPermissionsOptions.api.refreshView();
                                };
                                permission[property] = permissionValue;
                                if (permissionValue) {
                                    if (!permission[propertyToValidate + "Disabled"]) {
                                        permission[propertyToValidate] = false;
                                    }
                                    if (property === "isAllowed") {
                                        enableAllAllow();
                                        disableAllDeny();
                                    }
                                    else {
                                        enableAllDeny();
                                        disableAllAllow();
                                    }
                                }
                                else {
                                    if (property === "isAllowed") {
                                        disableAllAllow();
                                    }
                                    else {
                                        disableAllDeny();
                                    }
                                }
                            };
                            $scope.userOrGroupPermissionsOptions = {
                                angularCompileRows: true,
                                columnDefs: [
                                    {
                                        headerName: "Permissions for: ",
                                        field: "Name"
                                    },
                                    {
                                        headerName: "Allow",
                                        cellRenderer: function (params) {
                                            var permission = params.data;
                                            return "<input type='checkbox'" + (permission.isAllowed ? " checked='checked'" : "") + (!$scope.isEditable || permission.isAllowedDisabled ? " disabled='disabled'" : "") + " ng-click='HandlePermissionChange(data, \"isAllowed\", $event)'>";
                                        },
                                        width: 30,
                                        cellStyle: { 'text-align': 'center' }
                                    }, {
                                        headerName: "Deny",
                                        cellRenderer: function (params) {
                                            var permission = params.data;
                                            return "<input type='checkbox'" + (permission.isDenied ? " checked='checked'" : "") + (!$scope.isEditable || permission.isDeniedDisabled ? " disabled='disabled'" : "") + " ng-click='HandlePermissionChange(data, \"isDenied\", $event)'>";
                                        },
                                        width: 30,
                                        cellStyle: { 'text-align': 'center' }
                                    }, {
                                        headerName: "Inherited from",
                                        field: "InheritedFrom",
                                        cellRenderer: function (params) {
                                            var permissionInheritedFrom = params.data.InheritedFrom;
                                            return (permissionInheritedFrom ? "<span style='cursor: pointer;' title='" + permissionInheritedFrom + "'>" + permissionInheritedFrom + "</span>" : "");
                                        },
                                        width: 75
                                    }
                                ],
                                enableColResize: false,
                                enableSorting: false,
                                rowHeight: 25
                            };
                            permissionService.getUsersAndGroupsByFileId(model.value).then(function (resp) {
                                //console.log(resp);
                                $scope.usersAndGroups = resp.data.Data;
                                $scope.removedUsersAndGroupsPermissions = [];
                                $scope.usersAndGroupsPermissions = {};
                                $scope.userOrGroupPermissions = null;
                                $scope.usersAndGroupsOptions.api.setRowData($scope.usersAndGroups);
                                $scope.usersAndGroupsOptions.api.refreshView();
                            }, function (resp) {
                                //console.log(resp);
                            });
                        }]
                });
            }
            else {
                showAlert("", 2);
            }
        });
    };
    FileControllerDialogs.prototype.openTagDialog = function (model) {
        var _this = this;
        var that = this;
        if (!model)
            model = this.$scope.gridOptions.api.getSelectedRows()[0];
        var tagService = this.TagService;
        var permissionservice = this.PermissionService;
        var showAlert = this.$scope.ShowAlert;
        permissionservice.getEditTagPermission(model.value).then(function (resp) {
            if (resp && resp.data) {
                var isEditable = resp.data.Data;
                _this.ngDialog.openConfirm({
                    template: "modalMetadataDialogId",
                    className: "ngdialog-theme-default wider-dialog",
                    controller: ["$scope", function ($scope) {
                            $scope.folderOrFileName = model.name;
                            $scope.isEditable = isEditable;
                            $scope.tags = [];
                            $scope.removedTags = [];
                            $scope.selectedFile = model.value;
                            $scope.tagOptions = {
                                angularCompileRows: true,
                                columnDefs: [
                                    {
                                        headerName: "Tag",
                                        field: "TagName",
                                        editable: $scope.isEditable
                                    }, {
                                        headerName: "Value",
                                        field: "TagValue",
                                        editable: $scope.isEditable
                                    }, {
                                        headerName: "",
                                        cellRenderer: function (params) {
                                            return ($scope.isEditable ? "<button class='btn btn-xs btn-danger' ng-click='RemoveTagRow(data)'><i class='fa fa-minus' title='Remove row'></i></button>" : "");
                                        },
                                        width: 30,
                                        cellStyle: { 'text-align': 'center' }
                                    }
                                ],
                                rowData: $scope.tags,
                                enableColResize: false,
                                enableSorting: false,
                                rowHeight: 35,
                                onGridReady: function (event) {
                                    event.api.sizeColumnsToFit();
                                },
                                overlayNoRowsTemplate: "<span>No metadata found.</span>"
                            };
                            $scope.AddTagRow = function () {
                                if (isValid()) {
                                    $scope.tags.push({ TagName: "", TagValue: "" });
                                    $scope.tagOptions.api.setRowData($scope.tags);
                                    $scope.tagOptions.api.refreshView();
                                }
                            };
                            $scope.RemoveTagRow = function (tag) {
                                if ($scope.tags.length > 0) {
                                    var index = $scope.tags.indexOf(tag);
                                    if (index >= 0) {
                                        $scope.tags.splice(index, 1);
                                        if (tag.TagId) {
                                            $scope.removedTags.push(tag);
                                        }
                                        $scope.tagOptions.api.setRowData($scope.tags);
                                        $scope.tagOptions.api.refreshView();
                                    }
                                }
                            };
                            $scope.Save = function () {
                                if (isValid()) {
                                    var addedTags = [];
                                    var updatedTags = [];
                                    var lowereCaseKeies = [];
                                    var duplicateKeies = [];
                                    angular.forEach($scope.tags, function (tag) {
                                        var lowereCaseKey = tag.TagName.toLowerCase();
                                        if (lowereCaseKeies.indexOf(lowereCaseKey) === -1) {
                                            lowereCaseKeies.push(lowereCaseKey);
                                        }
                                        else {
                                            duplicateKeies.push(tag.TagName);
                                        }
                                        if (tag.TagId) {
                                            if (!angular.equals($scope.originalTags[tag.TagId], tag)) {
                                                updatedTags.push(tag);
                                            }
                                        }
                                        else {
                                            addedTags.push(tag);
                                        }
                                    });
                                    if (duplicateKeies.length > 0) {
                                        showAlert("Following duplicate tags found:<ul><li>" + duplicateKeies.join("</li><li>") + "</li></ul>", 2);
                                    }
                                    else {
                                        tagService.save($scope.selectedFile, { AddedTags: addedTags, UpdatedTags: updatedTags, RemovedTags: $scope.removedTags }).then(function (resp) {
                                            if (resp && resp.data) {
                                                if (resp.data.Data) {
                                                    $scope.closeThisDialog();
                                                    showAlert("Tags saved.", 1);
                                                    return;
                                                }
                                                else {
                                                    if (resp.data.Message) {
                                                        showAlert("Tags could not be saved due to following reason:<ul><li>" + resp.data.Message + "</li></ul>", 2);
                                                        return;
                                                    }
                                                    else {
                                                        showAlert("Tags could not be saved as you do not have permission", 2);
                                                        return;
                                                    }
                                                }
                                            }
                                            showAlert("Tags could not be saved.", 2);
                                        }, function (resp) {
                                            //console.log(resp);
                                        });
                                    }
                                }
                            };
                            var isValid = function () {
                                var tags = $scope.tags, allowToAddNewRow = true;
                                $("div#tagList div.ag-body-container div.ag-row").each(function (index, row) {
                                    var getValue = function (id) {
                                        var div = $(row).find("div.ag-cell[colid='" + id + "']");
                                        var input = div.find("input.ag-cell-edit-input");
                                        if (input.length > 0) {
                                            return input.val();
                                        }
                                        else {
                                            return div.text();
                                        }
                                    };
                                    var tag = tags[index];
                                    tag.TagName = getValue("TagName");
                                    tag.TagValue = getValue("TagValue");
                                });
                                for (var index = 0, count = tags.length; index < count; index++) {
                                    var tag = tags[index];
                                    if (tag.TagName === "" || tag.TagValue === "") {
                                        allowToAddNewRow = false;
                                        break;
                                    }
                                }
                                return allowToAddNewRow;
                            };
                            tagService.getByFileId(model.value).then(function (resp) {
                                //console.log(resp);
                                setTimeout(function () {
                                    $scope.tags = resp.data.Data;
                                    $scope.removedTags = [];
                                    $scope.originalTags = {};
                                    var tags = angular.copy($scope.tags);
                                    tags.forEach(function (tag) {
                                        $scope.originalTags[tag.TagId] = tag;
                                    });
                                    $scope.tagOptions.api.setRowData($scope.tags);
                                    $scope.tagOptions.api.refreshView();
                                });
                            }, function (resp) {
                                console.log(resp);
                            });
                        }]
                });
            }
            else {
                showAlert("", 2);
            }
        }, function (resp) {
            //console.log(resp);
        });
    };
    FileControllerDialogs.prototype.openAccountSelectionDialog = function (switchAccount) {
        var _this = this;
        var that = this;
        var dialog = this.ngDialog.open({
            template: 'modalAccountSelection',
            className: 'ngdialog-theme-default',
            showClose: false,
            closeByEscape: false,
            closeByDocument: false,
            controller: function ($scope) {
                $scope.switchAccount = switchAccount;
                //that.$rootScope.selectedAccount
            }
        });
        dialog.closePromise.then(function (data) {
            if (data && !data.value)
                return;
            //var userAccountId = data.value;           
            //sync user data on server
            _this.syncData();
        }, function (resp) {
            //console.log(resp);
            _this.syncData();
        });
    };
    FileControllerDialogs.prototype.syncData = function () {
        var that = this;
        that.FileService.SyncServerData().then(function (resp) {
            if (resp) {
                //Load file/Share list
                that.FileService.GetAllUserShares().then(function (resp) {
                    that.$scope.UserShares = [];
                    that.$scope["SelectedPath"] = "";
                    angular.forEach(resp.data.Data, function (item, index) {
                        that.$scope.UserShares.push({
                            folder: true,
                            open: false,
                            name: item["ShareName"],
                            value: item["FileEntryId"],
                            children: [],
                            CanWrite: item["CanWrite"],
                            fileShareId: item["FileShareId"],
                            isShare: true
                        });
                    });
                    that.RefreshFileExplorer();
                    var childItems = [];
                    if (!that.$scope.UserShares.length) {
                        that.RefreshMaingrid(childItems);
                        return;
                    }
                    that.OpenFolder(that.$scope.UserShares[0].value);
                    //childItems = that.$scope.UserShares[0]["children"];
                    //that.RefreshMaingrid(childItems);
                }, function (resp) {
                    //console.log(resp);
                });
            }
        }, function (resp) {
            //console.log(resp);
        });
    };
    FileControllerDialogs.prototype.OpenFileUploadDialog = function () {
        var that = this;
        if (this.$scope.gridOptions.api.getSelectedRows().length <= 0)
            return;
        var selectedFolderId = this.$scope.gridOptions.api.getSelectedRows()[0].value;
        var folderName = this.$scope.gridOptions.api.getSelectedRows()[0].name;
        var dialog = this.ngDialog.openConfirm({
            template: 'modalFileUploadDialog',
            className: 'ngdialog-theme-default',
            controller: ['$scope', '$window', '$timeout', 'md5', function ($scope, $window, $timeout, md5) {
                    // controller logic
                    //$scope.Header = "My custom title";
                    $scope.dropText = "Drop file here to upload";
                    $scope.selectedFile = null;
                    $scope.folderName = folderName;
                    var window = $window;
                    var maxBlockSize = 1 * 1024 * 1024; //Each file will be split in 1 MB.
                    var numberOfBlocks = 1;
                    //var selectedFile = null;
                    var currentFilePointer = 0;
                    var totalBytesRemaining = 0;
                    var blockIds = new Array();
                    var blockIdPrefix = "block-";
                    var submitUri = null;
                    var bytesUploaded = 0;
                    $scope.fileUploadUrl = "";
                    $scope.blobName = "";
                    $scope.isUploading = false;
                    //selected Folder 
                    var FileEntryId = selectedFolderId;
                    if (window.File && window.FileReader && window.FileList && window.Blob) {
                        // Great success! All the File APIs are supported.
                    }
                    else {
                        alert('The File APIs are not fully supported in this browser.');
                    }
                    //Upload button click
                    $scope.btnUploadClick = function () {
                        $scope.isUploading = true;
                        that.authInterceptorService.ShowLoader();
                        // Get Azure upload URL and Blob Name
                        that.FileService.GetBlobNameSasUrl(FileEntryId, $scope.selectedFile.size)
                            .then(function (resp) {
                            if (resp.data.Status !== 200) {
                                that.$scope.ShowAlert(resp.data.Message, 2);
                                $scope.isUploading = false;
                                that.authInterceptorService.HideLoader();
                                return;
                            }
                            var result = resp.data.Data;
                            $scope.fileUploadUrl = result.SharedAccessSignatureUrl;
                            $scope.blobName = result.BlobName;
                            $scope.progressVisible = true;
                            submitUri = result.SharedAccessSignatureUrl;
                            //console.log(submitUri);
                            that.FileService.AddFileAndCheckout(selectedFolderId, result.BlobName, $scope.selectedFile.name, $scope.selectedFile.size)
                                .then(function (resp) {
                                if (resp.data.Status !== 200) {
                                    that.$scope.ShowAlert(resp.data.Message, 2);
                                    $scope.isUploading = false;
                                    that.authInterceptorService.HideLoader();
                                    return;
                                }
                                $scope.fileModel = resp.data.Data;
                                //console.log('blobName-', $scope.blobName);
                                //uploadFileInBlocks();
                                //debugger
                                uploadFileInMultiThread();
                            }, function (resp) {
                                //console.log(resp);
                                $scope.isUploading = false;
                                that.authInterceptorService.HideLoader();
                            });
                        }, function (resp) {
                            //console.log(resp);
                            $scope.isUploading = false;
                            that.authInterceptorService.HideLoader();
                        });
                    };
                    //Read the file and find out how many blocks we would need to split it.
                    $scope.handleFileSelect = function (ctrl) {
                        var files = ctrl.files; // e.target.files;
                        //selectedFile = files[0];
                        setSelectedFile(files);
                    };
                    var setSelectedFile = function (files) {
                        currentFilePointer = 0;
                        totalBytesRemaining = 0;
                        bytesUploaded = 0;
                        $scope.$apply(function () {
                            $scope.selectedFile = files[0];
                            $scope.progress = 0;
                        });
                        var fileSize = $scope.selectedFile.size;
                        if (fileSize < maxBlockSize) {
                            maxBlockSize = fileSize;
                            console.log("max block size = " + maxBlockSize);
                        }
                        totalBytesRemaining = fileSize;
                        if (fileSize % maxBlockSize == 0) {
                            numberOfBlocks = fileSize / maxBlockSize;
                        }
                        else {
                            numberOfBlocks = parseInt((fileSize / maxBlockSize).toString(), 10) + 1;
                        }
                        console.log("total blocks = " + numberOfBlocks);
                    };
                    var uploadFileInMultiThread = function () {
                        // assert the browser support html5
                        // start to upload each files in chunks
                        var file = $scope.selectedFile; // files[i];
                        var fileSize = file.size;
                        var fileName = file.name;
                        // calculate the start and end byte index for each blocks(chunks)
                        // with the index, file name and index list for future using
                        //debugger
                        // define the function array and push all chunk upload operation into this array
                        //var async: any;
                        var blockSizeInKB = 500; //$("#block_size").val();
                        var blockSize = blockSizeInKB * 1024;
                        var blocks = [];
                        var offset = 0;
                        var index = 0;
                        var blockList = "";
                        console.log("total blocks in multi thread = " + fileSize / blockSize);
                        while (offset < fileSize) {
                            var start = offset;
                            var end = Math.min(offset + blockSize, fileSize);
                            var blockId = blockIdPrefix + pad(index, 6);
                            blockId = btoa(blockId);
                            blocks.push({
                                name: fileName,
                                blockId: blockId,
                                index: index,
                                start: start,
                                end: end
                            });
                            //list += index + ",";
                            blockList += '<Latest>' + blockId + '</Latest>';
                            offset = end;
                            index++;
                        }
                        // define the function array and push all chunk upload operation into this array
                        var putBlocks = [];
                        var uploadedChunks = 0;
                        blocks.forEach(function (block) {
                            putBlocks.push(function (callback) {
                                // load blob based on the start and end index for each chunks
                                var blob = file.slice(block.start, block.end);
                                var uri = submitUri + '&comp=block&blockid=' + block.blockId;
                                $.ajax({
                                    url: uri,
                                    type: "PUT",
                                    data: blob,
                                    processData: false,
                                    beforeSend: function (xhr) {
                                        xhr.setRequestHeader('x-ms-blob-type', 'BlockBlob');
                                        xhr.setRequestHeader('Content-Type', $scope.selectedFile.type);
                                        //xhr.setRequestHeader('Content-Length', requestData.length.toString());
                                    },
                                    success: function (data, status) {
                                        //console.log(data);
                                        //console.log(status);
                                        bytesUploaded += blob.size;
                                        var percentComplete = ((parseFloat(bytesUploaded.toString()) / parseFloat($scope.selectedFile.size)) * 100).toFixed(2);
                                        $scope.$apply(function () {
                                            $scope.progress = percentComplete;
                                        });
                                        if ((uploadedChunks++) === putBlocks.length - 1)
                                            callback(null, block.index);
                                    },
                                    error: function (xhr, desc, err) {
                                        console.log(desc);
                                        console.log(err);
                                        that.authInterceptorService.HideLoader();
                                    }
                                });
                            });
                        });
                        var commitCallback = function (error, result) {
                            //debugger
                            var uri = submitUri + '&comp=blocklist';
                            var requestBody = '<?xml version="1.0" encoding="utf-8"?><BlockList>';
                            requestBody += blockList;
                            //for (var i = 0; i < blockIds.length; i++) {
                            //    requestBody += '<Latest>' + blockIds[i] + '</Latest>';
                            //}
                            requestBody += '</BlockList>';
                            //console.log(requestBody);
                            $.ajax({
                                url: uri,
                                type: "PUT",
                                data: requestBody,
                                beforeSend: function (xhr) {
                                    xhr.setRequestHeader('x-ms-blob-content-type', $scope.selectedFile.type);
                                    //xhr.setRequestHeader('Content-Length', requestBody.length.toString());
                                },
                                success: function (data, status) {
                                    var readerCheckSum = new FileReader();
                                    readerCheckSum.addEventListener('load', function () {
                                        var fileHash = md5.createHash(this.result || '');
                                        //check-in file
                                        that.FileService.CheckIn($scope.fileModel.FileEntryId, $scope.fileModel.CurrentVersionNumber, $scope.selectedFile.size, $scope.blobName, fileHash)
                                            .then(function (resp) {
                                            if (resp.data.Data > 0) {
                                                that.ReGenerateMainGridData(null);
                                                $scope.closeThisDialog($scope.fileModel.FileEntryId);
                                                that.$scope.ShowAlert("File upload and check-in done successfully.", 1);
                                            }
                                            else
                                                that.$scope.ShowAlert("Error in File check-in. " + resp.data.Message, 2);
                                        }, function (resp) {
                                            console.log(resp);
                                            $scope.isUploading = false;
                                            that.authInterceptorService.HideLoader();
                                            that.$scope.ShowAlert(resp.data.Message, 2);
                                        });
                                    });
                                    //read file to get the checksum
                                    readerCheckSum.readAsBinaryString($scope.selectedFile);
                                    that.authInterceptorService.HideLoader();
                                },
                                error: function (xhr, desc, err) {
                                    console.log(desc);
                                    console.log(err);
                                    $scope.isUploading = false;
                                    that.authInterceptorService.HideLoader();
                                }
                            });
                        };
                        putBlocks.forEach(function (putBlock, index) {
                            putBlock.call(this, commitCallback);
                        });
                        // invoke the functions one by one
                        // then invoke the commit ajax call to put blocks into blob in azure storage
                        //async.parallel(putBlocks, commitCallback);
                    };
                    //var reader = new FileReader();
                    //reader.onabort = function (e) {
                    //    //btnUploadFile.prop("disabled", false);
                    //    alert("READING FILE OPERATION ABORTED!");
                    //};
                    //reader.onerror = function (e) {
                    //    //btnUploadFile.prop("disabled", false);
                    //    alert("ERROR OCCURRED IN READING FILE!");
                    //};
                    //reader.onloadend = function (evt) {
                    //    var readyState = evt.target["readyState"];
                    //    var FileReaderDone = FileReader["DONE"];
                    //    if (readyState == FileReaderDone) { // DONE == 2
                    //        var uri = submitUri + '&comp=block&blockid=' + blockIds[blockIds.length - 1];
                    //        var requestData = new Uint8Array(evt.target["result"]);
                    //        //ToDo: there is some issue Angular Put request, we are adding $ ajax request for it.
                    //        //that.FileService
                    //        //    .UploadBlob(uri, requestData)
                    //        //    .then((resp) => {
                    //        //        debugger;
                    //        //        //console.log(data);
                    //        //        //console.log(status);
                    //        //        bytesUploaded += requestData.length;
                    //        //        var percentComplete = ((parseFloat(bytesUploaded.toString()) / parseFloat($scope.selectedFile.size)) * 100).toFixed(2);
                    //        //        $scope.$apply(function () {
                    //        //            $scope.progress = percentComplete;
                    //        //        });
                    //        //        //$("#fileUploadProgress").text(percentComplete + " %");
                    //        //        uploadFileInBlocks();
                    //        //    }, (resp: any) => {
                    //        //        //console.log(resp);
                    //        //    });
                    //        $.ajax({
                    //            url: uri,
                    //            type: "PUT",
                    //            data: requestData,
                    //            processData: false,
                    //            beforeSend: function (xhr) {
                    //                xhr.setRequestHeader('x-ms-blob-type', 'BlockBlob');
                    //                xhr.setRequestHeader('Content-Type', $scope.selectedFile.type);
                    //                //xhr.setRequestHeader('Content-Length', requestData.length.toString());
                    //            },
                    //            success: function (data, status) {
                    //                //console.log(data);
                    //                //console.log(status);
                    //                bytesUploaded += requestData.length;
                    //                var percentComplete = ((parseFloat(bytesUploaded.toString()) / parseFloat($scope.selectedFile.size)) * 100).toFixed(2);
                    //                $scope.$apply(function () {
                    //                    $scope.progress = percentComplete;
                    //                });
                    //                uploadFileInBlocks();
                    //            },
                    //            error: function (xhr, desc, err) {
                    //                console.log(desc);
                    //                console.log(err);
                    //                that.authInterceptorService.HideLoader();
                    //            }
                    //        });
                    //    }
                    //};
                    //var uploadFileInBlocks = function () {
                    //    if (totalBytesRemaining > 0) {
                    //        console.log("current file pointer = " + currentFilePointer + " bytes read = " + maxBlockSize);
                    //        var fileContent = $scope.selectedFile.slice(currentFilePointer, currentFilePointer + maxBlockSize);
                    //        var blockId = blockIdPrefix + pad(blockIds.length, 6);
                    //        console.log("block id = " + blockId);
                    //        blockIds.push(btoa(blockId));
                    //        reader.readAsArrayBuffer(fileContent);
                    //        currentFilePointer += maxBlockSize;
                    //        totalBytesRemaining -= maxBlockSize;
                    //        if (totalBytesRemaining < maxBlockSize) {
                    //            maxBlockSize = totalBytesRemaining;
                    //        }
                    //    } else {
                    //        commitBlockList();
                    //    }
                    //};
                    //function commitBlockList() {
                    //    var uri = submitUri + '&comp=blocklist';
                    //    var requestBody = '<?xml version="1.0" encoding="utf-8"?><BlockList>';
                    //    for (var i = 0; i < blockIds.length; i++) {
                    //        requestBody += '<Latest>' + blockIds[i] + '</Latest>';
                    //    }
                    //    requestBody += '</BlockList>';
                    //    //console.log(requestBody);
                    //    $.ajax({
                    //        url: uri,
                    //        type: "PUT",
                    //        data: requestBody,
                    //        beforeSend: function (xhr) {
                    //            xhr.setRequestHeader('x-ms-blob-content-type', $scope.selectedFile.type);
                    //            //xhr.setRequestHeader('Content-Length', requestBody.length.toString());
                    //        },
                    //        success: function (data, status) {
                    //            var readerCheckSum = new FileReader();
                    //            readerCheckSum.addEventListener('load', function () {
                    //                var fileHash = md5.createHash(this.result || '');
                    //                //check-in file
                    //                that.FileService.CheckIn($scope.fileModel.FileEntryId, $scope.fileModel.CurrentVersionNumber, $scope.selectedFile.size, $scope.blobName, fileHash)
                    //                    .then((resp: any) => {
                    //                        if (resp.data.Data > 0) {
                    //                            that.ReGenerateMainGridData(null);
                    //                            $scope.closeThisDialog($scope.fileModel.FileEntryId);
                    //                            that.$scope.ShowAlert("File upload and check-in done successfully.", 1);
                    //                        }
                    //                        else
                    //                            that.$scope.ShowAlert("Error in File check-in. " + resp.data.Message, 2);
                    //                    }, (resp: any) => {
                    //                        console.log(resp);
                    //                        $scope.isUploading = false;
                    //                        that.$scope.ShowAlert(resp.data.Message, 2);
                    //                    });
                    //            });
                    //            //read file to get the checksum
                    //            readerCheckSum.readAsBinaryString($scope.selectedFile);
                    //            that.authInterceptorService.HideLoader();
                    //        },
                    //        error: function (xhr, desc, err) {
                    //            console.log(desc);
                    //            console.log(err);
                    //            $scope.isUploading = false;
                    //            that.authInterceptorService.HideLoader();
                    //        }
                    //    });
                    //}
                    function pad(number, length) {
                        var str = '' + number;
                        while (str.length < length) {
                            str = '0' + str;
                        }
                        return str;
                    }
                    $timeout(function () {
                        var scope = $scope;
                        var dropbox = document.getElementById("dropbox");
                        scope.dropText = 'Drop files here...';
                        // init event handlers
                        function dragEnterLeave(evt) {
                            evt.stopPropagation();
                            evt.preventDefault();
                            scope.$apply(function () {
                                scope.dropText = 'Drop files here...';
                                scope.dropClass = '';
                            });
                        }
                        dropbox.addEventListener("dragenter", dragEnterLeave, false);
                        dropbox.addEventListener("dragleave", dragEnterLeave, false);
                        dropbox.addEventListener("dragover", function (evt) {
                            evt.stopPropagation();
                            evt.preventDefault();
                            var clazz = 'not-available';
                            var ok = evt.dataTransfer && evt.dataTransfer.types && evt.dataTransfer.types["indexOf"]('Files') >= 0;
                            scope.$apply(function () {
                                scope.dropText = ok ? 'Drop files here...' : 'Only files are allowed!';
                                scope.dropClass = ok ? 'over' : 'not-available';
                            });
                        }, false);
                        dropbox.addEventListener("drop", function (evt) {
                            console.log('drop evt:', JSON.parse(JSON.stringify(evt.dataTransfer)));
                            evt.stopPropagation();
                            evt.preventDefault();
                            scope.$apply(function () {
                                scope.dropText = 'Drop files here...';
                                scope.dropClass = '';
                            });
                            var files = evt.dataTransfer.files;
                            setSelectedFile(files);
                            //if (files.length > 0) {
                            //    scope.$apply(function () {
                            //        scope.files = []
                            //        for (var i = 0; i < files.length; i++) {
                            //            scope.files.push(files[i])
                            //        }
                            //    })
                            //}
                        }, false);
                        //============== DRAG & DROP =============
                    }, 1000);
                }],
        });
    };
    FileControllerDialogs.prototype.OpenCreateFolderDialog = function (selectedNode) {
        if (selectedNode === void 0) { selectedNode = null; }
        var that = this;
        if (this.$scope.gridOptions.api.getSelectedRows().length <= 0)
            return;
        var selectedFolderId = this.$scope.gridOptions.api.getSelectedRows()[0].value;
        if (selectedNode) {
            var folderNode = this.GetFolderNode(selectedNode.value);
            selectedFolderId = folderNode.data.value;
        }
        var dialog = this.ngDialog.openConfirm({
            template: 'modalAddFolderDialog',
            className: 'ngdialog-theme-default',
            controller: ['$scope', '$window', function ($scope, $window) {
                    // controller logic
                    var window = $window;
                    $scope.name = "";
                    //Create button click
                    $scope.CreateFolderClick = function (e) {
                        //// Create new folder
                        that.FileService.CreateFolder(selectedFolderId, $scope.name)
                            .then(function (resp) {
                            if (resp.data.Data === false) {
                                that.$scope.ShowAlert(resp.data.Message, 2);
                                return;
                            }
                            that.RefreshGridsData(selectedFolderId, false);
                            $scope.closeThisDialog();
                            that.$scope.ShowAlert("Folder created successfully.", 1);
                            //console.log(resp);                           
                        }, function (resp) {
                            //console.log(resp);
                            that.$scope.ShowAlert("Error in folder create -" + resp.data.Message, 2);
                        });
                    };
                }],
        });
    };
    FileControllerDialogs.prototype.OpenAdvanceSearchDialog = function () {
        var that = this;
        if (this.$scope.gridOptions.api.getSelectedRows().length <= 0)
            return;
        var selectedFolderId = this.$scope.gridOptions.api.getSelectedRows()[0].value;
        var dialog = this.ngDialog.openConfirm({
            template: 'modalAdvanceSearchDialog',
            className: 'ngdialog-theme-default',
            controller: ['$scope', '$window', function ($scope, $window) {
                    // controller logic
                    var window = $window;
                    $scope.Filter = {};
                    $scope.Filter.tags = [{ TagName: "", TagValue: "" }];
                    $scope.Filter.StartFileId = selectedFolderId;
                    $scope.Filter.SearchText = that.$scope.Filter.SearchText || "";
                    $scope.Filter.IsAdvancedSearch = true;
                    $scope.tagOptions = {
                        columnDefs: [{ headerName: "Tag", field: "TagName", editable: true }, { headerName: "value", field: "TagValue", editable: true, onCellValueChanged: function (e) { } }],
                        rowData: $scope.Filter.tags,
                        enableColResize: false,
                        enableSorting: false,
                        rowHeight: 20,
                        onGridReady: function (event) {
                            event.api.sizeColumnsToFit();
                        }
                    };
                    var getTagData = function () {
                        var tags = [];
                        $("div#tagList div.ag-body-container div.ag-row").each(function (index, row) {
                            var getValue = function (id) {
                                var div = $(row).find("div.ag-cell[colid='" + id + "']");
                                if (div.find("input.ag-cell-edit-input").length > 0) {
                                    return div.find("input.ag-cell-edit-input").val();
                                }
                                else {
                                    return div.text();
                                }
                            };
                            var tag = { TagName: getValue("TagName"), TagValue: getValue("TagValue") };
                            if (tag.TagName !== "") {
                                tags.push(tag);
                            }
                        });
                        return tags;
                        //var tags = [];
                        //$scope.tagOptions.api.forEachNode(function (node, index) {
                        //    if (node.data.TagName !== "" && node.data.TagValue !== "") {
                        //        tags.push({ TagName: node.data.TagName, TagValue: node.data.TagValue });
                        //    }
                        //});
                        //return tags;
                    };
                    $scope.AddTagRow = function (e) {
                        //$scope.tagOptions.api.stopEditing();
                        $scope.Filter.tags = getTagData();
                        var allowToAddNewRow = true;
                        angular.forEach($scope.Filter.tags, function (item, index) {
                            if (item.TagName === "" || item.TagValue === "") {
                                allowToAddNewRow = false;
                            }
                        });
                        if (!allowToAddNewRow)
                            return;
                        $scope.Filter.tags.push({ TagName: "", TagValue: "" });
                        $scope.tagOptions.api.setRowData($scope.Filter.tags);
                        $scope.tagOptions.api.refreshView();
                    };
                    //Search button click
                    $scope.SearchClick = function (e) {
                        $scope.Filter.tags = getTagData();
                        if ($scope.Filter.tags.length > 0 || $scope.Filter.SearchText != "") {
                            that.$scope.Filter = $scope.Filter || {};
                            var filter = that.$scope.Filter || {};
                            //filter.tags = that.$scope.Filter.tags || null;
                            //filter.SearchText = that.$scope.Filter.SearchText || "";
                            //filter.IsAdvancedSearch = true;
                            that.ReGenerateMainGridData(filter);
                            $scope.closeThisDialog();
                        }
                    };
                }],
        });
    };
    return FileControllerDialogs;
}(FileControllerGrid));
//scope and rootscope items are defined here
var FileController$Scope = /** @class */ (function (_super) {
    __extends(FileController$Scope, _super);
    function FileController$Scope(FileService, ngDialog, OrganizationService, PermissionService, TagService, $scope, $rootScope, $timeout, $compile, $filter, authInterceptorService) {
        var _this = _super.call(this, FileService, ngDialog, OrganizationService, PermissionService, TagService, $scope, $rootScope, $timeout, $compile, $filter, authInterceptorService) || this;
        _this.FileService = FileService;
        _this.ngDialog = ngDialog;
        _this.OrganizationService = OrganizationService;
        _this.PermissionService = PermissionService;
        _this.TagService = TagService;
        _this.$scope = $scope;
        _this.$rootScope = $rootScope;
        _this.$timeout = $timeout;
        _this.$compile = $compile;
        _this.$filter = $filter;
        _this.authInterceptorService = authInterceptorService;
        var that = _this;
        $scope.IsAdmin = false;
        $rootScope.IsAccountAdmin = false;
        $rootScope.setUser = function (user) {
            //alert('2');
            try {
                localStorage["UserDetail"] = JSON.stringify(user);
            }
            catch (e) { }
            $rootScope.userDetail = user;
            $rootScope.userDetail.userName = $rootScope.userDetail.UniqueName;
            //debugger;
            PermissionService.isAccountAdmin().then(function (resp) {
                if (resp && resp.data) {
                    $rootScope.IsAccountAdmin = resp.data.Data;
                }
            }, function (resp) {
                $rootScope.IsAccountAdmin = false;
            });
        };
        $rootScope.getUser = function () {
            //alert('NEERAJ SHARMA');
            if (!$rootScope.accountDetails) {
                if (!$rootScope.userDetail) {
                    try {
                        $rootScope.userDetail = JSON.parse(localStorage["UserDetail"]);
                        $rootScope.userDetail.userName = $rootScope.userDetail.UniqueName;
                        //that.$rootScope.IsAccountAdmin = $rootScope.IsAccountAdmin;
                        that.$scope.IsAdmin = $rootScope.userDetail && $rootScope.userDetail.IsAdmin === true;
                        PermissionService.isAccountAdmin().then(function (resp) {
                            if (resp && resp.data) {
                                $rootScope.IsAccountAdmin = resp.data.Data;
                            }
                        }, function (resp) {
                            $rootScope.IsAccountAdmin = false;
                        });
                    }
                    catch (e) { }
                }
                return $rootScope.userDetail;
            }
            $rootScope.getUser();
        };
        _this.$scope.split_bar_mousedown = function (e) {
            e.preventDefault();
            var grid = this;
            $(document).mousemove(function (e) {
                e.preventDefault();
                var x = e.pageX - $('#sidebar').offset().left;
                if (x > min && x < max && e.pageX < ($(window).width() - mainmin)) {
                    $('#sidebar').css("width", x);
                    $('#main').css("margin-left", x);
                    console.log("$scope", grid);
                    grid.gridOptions.api.sizeColumnsToFit();
                    grid.gridOptions.api.doLayout();
                }
            });
        };
        _this.$scope.ShowAlert = function (msg, alertType) {
            //supress alerts for success
            if (parseInt(alertType) === 1)
                return;
            var dialog = that.ngDialog.openConfirm({
                template: 'modalAlertDialog',
                className: 'ngdialog-theme-default',
                controller: ['$scope', function ($scope) {
                        // controller logic
                        $scope.alertMessage = msg;
                        $scope.alertClass = "";
                        if (parseInt(alertType) === 1) //success
                         {
                            $scope.alertClass = "alert alert-success";
                        }
                        else if (parseInt(alertType) === 2) //Error
                         {
                            $scope.alertClass = "alert alert-danger";
                        }
                    }],
            });
        };
        _this.$scope.openHistoryDialog = function (id) {
            var dialog = that.ngDialog.openConfirm({
                template: 'modalHistoryDialog',
                className: 'ngdialog-theme-default wider-dialog',
                //scope:$scope,
                controller: ['$scope', function ($scope) {
                        // controller logic
                        $scope.id = id;
                        that.FileService.GetVersionHistory(id)
                            .then(function (resp) {
                            if (resp.data.Status == 404) {
                                return;
                            }
                            var historyData = resp.data.Data;
                            angular.forEach(historyData, function (item, index) {
                                item.Time = new Date(item.Time).toLocaleString("en-IN");
                            });
                            var historyRowData = [];
                            angular.forEach(resp.data.Data, function (item, index) {
                                historyRowData.push({
                                    name: item["FileEntryNameWithExtension"],
                                    value: item["FileEntryId"],
                                    version: item["Version"],
                                    FilePath: item["FilePath"],
                                    Time: item["Time"],
                                    User: item["User"]
                                });
                            });
                            $scope.HistoryDataOptions.api.setRowData(historyRowData);
                            $scope.HistoryDataOptions.api.sizeColumnsToFit();
                        }, function (resp) {
                            //console.log(resp);
                        });
                        var tooltipRenderer = function (params) {
                            return '<span title="' + params.value + '">' + params.value + '</span>';
                        };
                        //Set the grid columns properties
                        var columnDefs = [
                            { headerName: "Version", field: "version", width: 75, cellRenderer: tooltipRenderer },
                            { headerName: "Path", field: "FilePath", width: 250, cellRenderer: tooltipRenderer },
                            { headerName: "Date", field: "Time", width: 150, cellRenderer: tooltipRenderer },
                            { headerName: "User", field: "User", width: 250, cellRenderer: tooltipRenderer },
                            {
                                headerName: "Action", field: "", width: 150,
                                cellRenderer: function (params) {
                                    if (params.data.version <= 0)
                                        return '';
                                    var buttonStr = '';
                                    buttonStr +=
                                        '<button id= "btn" class="btn btn-primary margin-left-15 btn-xs"  title= "Download file" ' + " ng-click='DownloadHistoryGridActionClick(\"Download\", $event,  \"" + params.data.value + "\" , \"" + params.data.version + "\")' >" +
                                            '<i class="glyphicon glyphicon-cloud-download" > </i>' +
                                            '</button>';
                                    return buttonStr;
                                }
                            }
                        ];
                        $scope.HistoryDataOptions = {
                            columnDefs: columnDefs,
                            rowData: null,
                            rowSelection: 'single',
                            enableColResize: true,
                            suppressMovableColumns: false,
                            enableSorting: true,
                            rowHeight: 35,
                            suppressRowClickSelection: false,
                            angularCompileRows: true,
                            suppressLoadingOverlay: true,
                            suppressCellSelection: true,
                            onGridReady: function (event) {
                                event.api.sizeColumnsToFit();
                            },
                            overlayNoRowsTemplate: '<span style="">No history exists.</span>',
                            onRowSelected: function (e) {
                                //console.log("onRowSelected, ", e);
                                var key = e.node.data.value;
                            }
                        };
                        $scope.DownloadHistoryGridActionClick = function (action, e, fileId, version) {
                            //console.log(action, e, fileId);
                            if (action === "Download") {
                                $scope.HistoryDataOptions.api.forEachNode(function (node, index) {
                                    //make grid row selected when click button
                                    if (node.data.value === fileId && node.data.version === version) {
                                        node.setSelected(true, true);
                                    }
                                });
                                var selectedRow = $scope.HistoryDataOptions.api.getSelectedRows()[0];
                                if (selectedRow)
                                    that.DownloadFile(selectedRow);
                            }
                        };
                    }],
            });
        };
        _this.$scope.UndoCheckout = function () {
            var selectedRows = that.$scope.maingridOptions.api.getSelectedRows();
            if (!selectedRows.length)
                return;
            if (!selectedRows[0]["CheckedOutBy"]) { //File Already checked in 
                that.$scope.ShowAlert("Already Checked-in", 2);
                return;
            }
            var id = selectedRows[0]["value"];
            that.FileService.UndoCheckout(id)
                .then(function (resp) {
                if (resp.data.Data === true) {
                    that.ReGenerateMainGridData(null);
                    //that.$scope.ShowAlert("Undo checkout done.", 1);
                }
                else
                    that.$scope.ShowAlert("Error in undo checkout. " + resp.data.Message, 2);
            }, function (resp) {
                //console.log(resp);
            });
        };
        _this.$scope.CheckOut = function () {
            //var key = that.$scope.SelectedFolder;
            var selectedRowsLeftGrid = that.$scope.gridOptions.api.getSelectedRows();
            if (!selectedRowsLeftGrid.length)
                return;
            var key = selectedRowsLeftGrid[0]["value"];
            var selectedRows = that.$scope.maingridOptions.api.getSelectedRows();
            if (!selectedRows.length)
                return;
            if (selectedRows[0]["CheckedOutBy"]) { //File Already checked out
                that.$scope.ShowAlert("Already Checked out", 2);
                return;
            }
            var id = selectedRows[0]["value"];
            var version = selectedRows[0]["version"];
            //console.log("getSelectedRows() ", that.$scope.maingridOptions.api.getSelectedRows());
            that.FileService.CheckOut(id, version)
                .then(function (resp) {
                if (resp.data.Data === false) {
                    that.$scope.ShowAlert(resp.data.Message, 2);
                    return;
                }
                //refresh grid data
                that.ReGenerateMainGridData(null);
                that.$scope.ShowAlert("Checkout done", 1);
            }, function (resp) {
                //console.log(resp);
            });
        };
        _this.$scope.buttonClick = function (key, e) {
            switch (key) {
                case "history":
                    var id = this.maingridOptions.api.getSelectedRows()[0].value;
                    this.openHistoryDialog(id);
                    break;
                case "checkout":
                    this.CheckOut();
                    break;
                case "undocheckout":
                    this.UndoCheckout();
                    break;
                case "uploadfile":
                    that.OpenFileUploadDialog();
                    break;
                case "downloadfile":
                    var selectedRow = this.maingridOptions.api.getSelectedRows()[0];
                    that.DownloadFile(selectedRow);
                    break;
                case "clearsearch":
                    that.ClearSearch();
                    break;
                case "advancesearch":
                    that.OpenAdvanceSearchDialog();
                    break;
                case "createfolder":
                    that.OpenCreateFolderDialog();
                    break;
                case "openparentfolder":
                    that.OpenParentFolder();
                    break;
                case "refresh":
                    that.RefreshPage();
                    break;
                case "logout":
                    that.Logout();
                    break;
                case "deletedFiles":
                    that.OpenDeletedFileDialog();
                    break;
                case "moveFile":
                    var selectedRow = this.maingridOptions.api.getSelectedRows()[0];
                    that.OpenMoveDialog(selectedRow);
                    break;
                case "setPermissions":
                    var selectedRow = this.maingridOptions.api.getSelectedRows()[0];
                    that.openPermissionDialog(selectedRow);
                    break;
                case "setMetaData":
                    var selectedRow = this.maingridOptions.api.getSelectedRows()[0];
                    that.openTagDialog(selectedRow);
                    break;
                case "deleteFile":
                    var selectedRow = this.maingridOptions.api.getSelectedRows()[0];
                    that.DeleteRestorePurgeDialog(selectedRow, "Delete");
                    break;
            }
        };
        return _this;
    }
    FileController$Scope.prototype.ClearSearch = function () {
        var that = this;
        that.$scope.Filter.SearchText = '';
        that.ReGenerateMainGridData(null);
    };
    FileController$Scope.prototype.Logout = function () {
        //localStorage.removeItem("SelectedAccount");
        localStorage.removeItem("UserDetail");
        this.$rootScope.selectedAccount = null;
        this.$rootScope.isUserAuthentic = false;
        this.$rootScope.userDetail = null;
        this.$scope.UserShares = [];
        this.authInterceptorService.Logout();
    };
    FileController$Scope.prototype.RefreshPage = function () {
        var that = this;
        var selectedRowsLeftGrid = that.$scope.gridOptions.api.getSelectedRows();
        if (!selectedRowsLeftGrid.length)
            return;
        var key = selectedRowsLeftGrid[0]["value"];
        var folderNode = this.GetFolderNode(key);
        //create filter params       
        that.$scope.maingridOptions.columnApi.setColumnVisible('path', false);
        var refreshContent = function () {
            key = folderNode.data.value;
            that.FileService.GetAllChildren({ id: key, filter: null })
                .then(function (resp) {
                if (resp.data.Status == 404) {
                    folderNode = folderNode.parent;
                    if (folderNode != null)
                        refreshContent();
                    return;
                }
                var respData = [];
                angular.forEach(resp.data.Data, function (item, index) {
                    respData.push(item);
                });
                var folderNodes = [];
                var allChildNodes = that.FileService.ConvertToMainGridModel(respData);
                angular.forEach(resp.data.Data, function (item, index) {
                    if (item["FileEntryTypeId"] !== 2) //Only folder needs to list in Left 
                        return;
                    folderNodes.push({
                        folder: true,
                        open: false,
                        name: item["FileVersion"]["FileEntryName"],
                        value: item["FileEntryId"],
                        children: [],
                        CanWrite: item["CanWrite"],
                        fileShareId: item["FileShareId"],
                        currentVersionNumber: item["CurrentVersionNumber"],
                        isDeleted: item["IsDeleted"] || item["FileVersion"]["IsDeleted"]
                    });
                });
                folderNode.data.children = folderNodes;
                that.RefreshFileExplorer();
                folderNode = that.GetFolderNode(key);
                folderNode.setSelected(true, true);
                that.$scope.maingridOptions.columnApi.setColumnVisible('path', false);
                that.RefreshMaingrid(allChildNodes);
            }, function (resp) {
                //console.log(resp);
            });
        };
        refreshContent();
    };
    return FileController$Scope;
}(FileControllerDialogs));
//main controller, consuming above classes and binding data to UI
var FileController = /** @class */ (function (_super) {
    __extends(FileController, _super);
    function FileController(FileService, ngDialog, OrganizationService, PermissionService, TagService, $scope, $rootScope, $timeout, $compile, $filter, authInterceptorService) {
        var _this = _super.call(this, FileService, ngDialog, OrganizationService, PermissionService, TagService, $scope, $rootScope, $timeout, $compile, $filter, authInterceptorService) || this;
        _this.FileService = FileService;
        _this.ngDialog = ngDialog;
        _this.OrganizationService = OrganizationService;
        _this.PermissionService = PermissionService;
        _this.TagService = TagService;
        _this.$scope = $scope;
        _this.$rootScope = $rootScope;
        _this.$timeout = $timeout;
        _this.$compile = $compile;
        _this.$filter = $filter;
        _this.authInterceptorService = authInterceptorService;
        _this.getSelectedFolderModel = function () {
            var selectedFolder = this.$scope.gridOptions.api.getSelectedRows()[0];
            if (selectedFolder)
                return selectedFolder;
            return {};
        };
        _this.gridPanelMenuOptions = function () {
            this.$scope.maingridOptions.api.deselectAll();
            this.$scope.gridOptions.api.setFocusedCell();
            var selectedFolder = this.getSelectedFolderModel();
            if (selectedFolder)
                return this.menuOptionsTree(selectedFolder);
            return [];
        };
        var that = _this;
        //todo: remove below line after final testing
        //$scope.IsAdmin = true;
        $rootScope.setSelectedAccount = function (account) {
            try {
                //debugger;
                //localStorage["SelectedAccount"] = JSON.stringify(account);
                $rootScope.accountDetails = JSON.stringify(account);
                var domainName = account.accountName;
                var baseUrl = window.location.href;
                var dName = baseUrl.split("//");
                var isChkDomain = dName[1].match(domainName);
                //alert('423432');
                //debugger;
                if (!isChkDomain) {
                    var baseUrl = dName[0] + "//" + domainName + "." + dName[1];
                    localStorage.setItem("serviceUrl", baseUrl);
                    _this.$rootScope.isUserAuthentic = true;
                    //Location.$inject()
                    location.assign(baseUrl);
                    //location.replace(baseUrl);
                    return false;
                }
            }
            catch (e) { }
            $rootScope.selectedAccount = account;
        };
        _this.OrganizationService.GetUserLogedIn()
            .then(function (resp) {
            if (resp.data.Data) {
                _this.$rootScope.isUserAuthentic = true;
                _this.setAccount();
            }
            else {
                _this.$rootScope.isUserAuthentic = false;
            }
        });
        //this.getAllFiles();
        _this.$scope.UserShares = [];
        _this.LoadFileExplorer();
        _this.$scope.SelectedFolder = "";
        $timeout(function () {
            $(document).on("contextmenu", function (e) {
                e.preventDefault();
                e.stopPropagation();
            });
            //if (!FileController.DropzoneAttached) {
            //    that.AttachDropzone();
            //    FileController.DropzoneAttached = true;
            //}
        });
        _this.$scope.IsFileORFolderSelected = function () {
            return (this.maingridOptions.api.getSelectedRows().length > 0);
        };
        _this.$scope.IsShareSelected = function () {
            var rows = this.gridOptions.api.getSelectedRows();
            if (rows.length == 0)
                return false;
            return rows[0].isShare;
        };
        _this.$scope.IsFileSelected = function () {
            return !(this.maingridOptions.api.getSelectedRows().length == 0 || this.maingridOptions.api.getSelectedRows()[0].folder === true || this.maingridOptions.api.getSelectedRows()[0].version === 0);
        };
        _this.$scope.IsOnlyOneFileSelected = function () {
            var selectedRows = this.maingridOptions.api.getSelectedRows();
            var files = selectedRows.filter(function (item) { return !item.folder && item.version > 0; });
            return selectedRows.length === 1 && selectedRows.length === files.length;
        };
        _this.$scope.IsFileAndFolderSelected = function () {
            var folders = this.maingridOptions.api.getSelectedRows().filter(function (item) { return item.folder === true; });
            return this.maingridOptions.api.getSelectedRows().length > 1 && folders.length === 0; // this.maingridOptions.api.getSelectedRows()[0].folder === true;
        };
        _this.$scope.IsWrite = function () {
            if (this.maingridOptions.api.getSelectedRows().length == 0)
                return false;
            var item = this.maingridOptions.api.getSelectedRows()[0];
            return (item.folder === false && item.CanWrite === true && item.version > 0) || (item.folder === true && item.CanWrite === true);
        };
        _this.$scope.IsCheckedOut = function () {
            if (this.maingridOptions.api.getSelectedRows().length == 0)
                return false;
            var item = this.maingridOptions.api.getSelectedRows()[0];
            return item.folder === false && item.CheckedOutBy !== null && item.CheckedOutBy !== "";
        };
        _this.$scope.IsAuthorizedToUndoCheckedOut = function () {
            if (!that.$scope.IsOnlyOneFileSelected())
                return false;
            var item = this.maingridOptions.api.getSelectedRows()[0];
            return item.folder === false && item.CanWrite === true && item.version > 0 && item.CheckedOutBy && (that.$rootScope.userDetail.userName === item.CheckedOutBy || that.$rootScope.IsAccountAdmin);
        };
        _this.$scope.IsFolderSelected = function () {
            if (this.gridOptions.api.getSelectedRows().length == 0)
                return false;
            return !(this.gridOptions.api.getSelectedRows()[0].CanWrite === false);
        };
        _this.$scope.IsBlobContainerSelected = function () {
            if (this.gridOptions.api.getSelectedRows().length == 0)
                return false;
            return true;
        };
        _this.$scope.Filter = { tags: null, SearchText: "", IsAdvancedSearch: false };
        _this.$scope.searchChanged = false;
        _this.$scope.SearchChange = function () {
            that.$scope.searchChanged = true;
        };
        _this.$scope.FileSearch = function () {
            that.$scope.Filter.tags = null;
            that.$scope.Filter.IsAdvancedSearch = false;
            var filter = that.$scope.Filter;
            if (!that.$scope.searchChanged && (!filter || !filter.SearchText))
                return;
            that.$scope.searchChanged = false;
            that.ReGenerateMainGridData(filter);
        };
        return _this;
    }
    FileController.prototype.$onInit = function () { };
    FileController.prototype.GotoLogin = function () {
        this.authInterceptorService.GotoLogin();
    };
    FileController.prototype.AttachDropzone = function () {
        var that = this;
        var fileUploadUrl = "/file/post";
        $("div#mainFileBrowser .ag-body-viewport").dropzone({
            url: fileUploadUrl,
            clickable: false,
            uploadMultiple: true,
            acceptedFiles: "image/*,application/pdf,.psd",
            drop: function (e) {
                //console.log("File Dropped: ", e);
            },
            addedfile: function (file) {
                //console.log("file added: ", file);
                var isFileExists = false;
                that.$scope.maingridOptions.api.forEachNode(function (node) {
                    if (isFileExists)
                        return;
                    var data = node.data;
                    if (data.name == file.name) { // we found a group node!!!
                        isFileExists = true;
                    }
                });
                //file.previewElement = Dropzone.createElement(this.options.previewTemplate);
                // Now attach this new element some where in your page
            },
        });
    };
    FileController.prototype.setAccount = function () {
        var _this = this;
        this.OrganizationService.GetAccountName()
            .then(function (resp) {
            if (resp.data.Data) {
                var acct = { accountId: resp.data.Data.AccountId, accountName: resp.data.Data.AccountName };
                _this.$rootScope.setSelectedAccount(acct);
                var organizationService_1 = _this.OrganizationService;
                var rootScope_1 = _this.$rootScope;
                var that_1 = _this;
                _this.OrganizationService.GetServiceURL(acct.accountId).then(function (resp) {
                    if (resp.data[0].servicerURL) {
                        localStorage.setItem("serviceUrl", resp.data[0].serviceUrl);
                        localStorage.setItem("servicerURL", resp.data[0].servicerURL);
                        organizationService_1.GetUserDetail()
                            .then(function (res) {
                            //alert('1');
                            //debugger;
                            rootScope_1.setUser(res.data);
                            that_1.syncData();
                        }, function (res) {
                            console.log(res);
                        });
                    }
                });
            }
            else {
                //debugger
                _this.$rootScope.isUserAuthentic = true;
                _this.openAccountSelectionDialog(false);
            }
        }, function (resp) {
            console.log(resp);
        });
    };
    FileController.$ = ['JQuery'];
    return FileController;
}(FileController$Scope));
angular.module('cloudSync').controller('FileController', FileController);
//# sourceMappingURL=FileController.js.map