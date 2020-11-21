
"use strict";
agGrid.initialiseAgGridWithAngular1(angular);

var serviceBase: String = 'http://192.168.5.100:44301/';

var app = angular.module('cloudSync', ['ngDialog', 'ui.bootstrap.contextMenu', 'agGrid', 'ngMd5', 'ngSanitize'])

    .config(function ($httpProvider: ng.IHttpProvider) {
        $httpProvider.interceptors.push('authInterceptorService');
    });

