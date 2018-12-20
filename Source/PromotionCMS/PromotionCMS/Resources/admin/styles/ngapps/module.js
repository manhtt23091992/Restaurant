var vnpApp = angular.module("vnpApp", ["ui.bootstrap", "ngSanitize", "ngRoute"]);

vnpApp.factory("ShareData", function () {
    return { value: 0 }
});

//Showing Routing
vnpApp.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
    $routeProvider.when('/admin/showallpage',
                        {
                            templateUrl: 'admin/Page/ShowAllPage',
                            controller: 'PageApiController'
                        });
    $routeProvider.otherwise(
                        {
                            redirectTo: '/'
                        });

    $locationProvider.html5Mode({
        enabled: true,
        requireBase: false
    });
}]);