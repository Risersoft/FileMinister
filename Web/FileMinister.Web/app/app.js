"use strict";
agGrid.initialiseAgGridWithAngular1(angular);
var serviceBase = 'http://192.168.5.100:44301/';
var app = angular.module('cloudSync', ['ngDialog', 'ui.bootstrap.contextMenu', 'agGrid', 'ngMd5', 'ngSanitize'])
    .config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});
//# sourceMappingURL=app.js.map