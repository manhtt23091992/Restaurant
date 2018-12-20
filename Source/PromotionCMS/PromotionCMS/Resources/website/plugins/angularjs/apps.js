var vnpApp = angular.module("vnpApp", ["ngSanitize", "angularLoad", "ui.bootstrap", "ngTouch", "angucomplete-alt", "ng.deviceDetector"]);
//Filter
vnpApp.filter('formatdate', [
    '$filter', function ($filter) {
        return function (input, format) {
            return $filter('date')(new Date(input), format);
        };
    }
]);



vnpApp.filter('formatjsondate', [
    '$filter', function ($filter) {
        return function (input, format) {
            var date = new Date(parseInt(input.substr(6)));
            return $filter('date')(new Date(date), format);
        };
    }
]);

vnpApp.filter("strLimit", ["$filter", function ($filter) {
    return function (input, limit) {
        if (input === null) {
            input = "";
        }
        if (input.length <= limit) {
            return input;
        }

        return $filter("limitTo")(input, limit) + '...';
    };
}]);

vnpApp.directive("ngDatePicker", function () {
    return {
        link: function (scope, element, attr) {
            var target = $(element);
            target.datetimepicker({
                format: 'd-m-Y',
                timepicker: false,
                mask: true,
                maxDate: '',
                onSelectDate: function (currentDateTime) {
                    if (target.is('[ng-from-date]')) {
                        var endDatePicker = $("input[ng-end-date]");
                        //set date
                        var fromDateString = currentDateTime.getFullYear() + "-" + ((currentDateTime.getMonth() + 1) < 10 ? '0' : '') + (currentDateTime.getMonth() + 1) + "-" + (currentDateTime.getDate() < 10 ? '0' : '') + currentDateTime.getDate();
                        //console.log("fromDateString " + fromDateString.replace(/-/g, '/'));
                        var _fromDate = Date.parse(fromDateString);
                        //console.log("fromdate " + _fromDate);

                        var toDateString = endDatePicker.val();
                        //console.log("toDateString " + toDateString.replace(/-/g, '/'));
                        var _toDate = Date.parse(toDateString);
                        //console.log("todate " + _toDate);

                        if (_fromDate > _toDate) {
                            endDatePicker.val(fromDateString);
                        }

                        endDatePicker.datetimepicker({
                            minDate: fromDateString.replace(/-/g, '/'),
                            maxDate: ''
                        });
                        endDatePicker.focus();
                    }
                    else if (target.is('[ng-end-date]')) {
                        var startDatePicker = $("input[ng-from-date]");
                        //set date
                        var toDateString = currentDateTime.getFullYear() + "-" + ((currentDateTime.getMonth() + 1) < 10 ? '0' : '') + (currentDateTime.getMonth() + 1) + "-" + (currentDateTime.getDate() < 10 ? '0' : '') + currentDateTime.getDate();
                        //console.log("toDateString " + toDateString.replace(/-/g, '/'));
                        var _toDate = Date.parse(toDateString);
                        //console.log("toDate " + _toDate);

                        var fromDateString = startDatePicker.val();
                        //console.log("fromDateString " + fromDateString.replace(/-/g, '/'));
                        var _fromDate = Date.parse(fromDateString);
                        //console.log("fromdate " + _fromDate);

                        if (_fromDate > _toDate) {
                            startDatePicker.val(toDateString);
                        }

                        startDatePicker.datetimepicker({
                            maxDate: toDateString.replace(/-/g, '/')
                        });
                    }
                }
            });
        }
    };
});

vnpApp.controller('HomeController', ["$scope", 'deviceDetector', function($scope, deviceDetector) {
        $scope.deviceDetector = deviceDetector;
    }
]);
vnpApp.controller('NewsController', ["$scope", function ($scope) {
    $scope.ListData = [];
    $scope.ObjNews = null;

    $scope.totalRow = 0;
    $scope.itemsPerPage = 8;
    $scope.currentPage = 1;
    $scope.maxSize = 6;

    $scope.module = "";

    $scope.title = "";
    $scope.isPost = "0";

    //End init
    $scope.GetListPost = function () {
        setTimeout(function () {
            $.ajax({
                url: "/get-post",
                data: {
                    pageNo: $scope.currentPage
                },
                type: "POST",
                beforeSend: function () {
                    $(".loading").show();
                },
                complete: function () {
                    $(".loading").hide();
                },
                success: function (data) {
                    if (data.ListData.length > 0) {
                        $scope.totalRow = data.TotalRow;
                        if ($scope.currentPage === 1) {
                            $scope.ObjNews = data.ListData[0];
                        } else {
                            $scope.ObjNews = null;
                        }

                    } else {
                        $scope.totalRow = 0;
                    }
                    var lazy = Lazy(data.ListData);

                    if ($scope.currentPage !== 1) {
                        $scope.ListData = data.ListData;
                    } else {
                        $scope.ListData = lazy.skip(1).toArray();
                    }

                    console.log($scope.ListData);
                    console.log($scope.ObjNews);
                    if (!$scope.$$phase)
                        $scope.$apply();
                },
                error: ""
            });
        }, 500);

    };

    //===========Paging Bootstrap=====
    $scope.pageChanged = function () {
        $scope.GetListPost();
        $("html, body").animate({ scrollTop: 0 }, "slow");
    };
}]);
