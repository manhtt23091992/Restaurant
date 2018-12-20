vnpApp.controller('PageApiController', function ($scope, $location, VnpayService, ShareData) {
    initPage();

    function initPage() {
        var sPage = VnpayService.getPages();
        sPage.then(function (pl) { $scope.page = pl.data; },
              function (errorPl) {
                  $scope.error = errorPl;
              });
    }
});