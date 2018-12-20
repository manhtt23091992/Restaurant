var vnpApp = angular.module("vnpApp", ["ui.bootstrap", "ngSanitize", "ngRoute"]);
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

angular.element(document).ready(function () {
});

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

vnpApp.directive("datepicker", function () {
        return {
            restrict: "A",
            require: "ngModel",
            link: function (scope, elem, attrs, ngModelCtrl) {
                var updateModel = function () {
                    scope.$apply(function () {
                        ngModelCtrl.$modelValue = elem.val();
                    });
                };
                elem.datetimepicker({
                    useCurrent: false,
                    minuteStepping: 5,
                    icons: {
                        time: 'fa fa-clock-o',
                        date: 'fa fa-calendar',
                        up: 'fa fa-arrow-up',
                        down: 'fa fa-arrow-down'
                    }
                });
                elem.on("change", function (e) {
                    updateModel();
                });
            }
        }
    });

vnpApp.controller('UserManagerController', ["$scope", function ($scope) {
    $scope.RspList = [];
    $scope.totalRow = 0;
    $scope.itemsPerPage = 12;
    $scope.currentPage = 1;
    $scope.maxSize = 6;
    //Init variable
    $scope.username = "";
    $scope.roleId = 0;
    $scope.userId = "0";
    $scope.isLocked = "";
    //End init

    $scope.SetUser = function (userId, isLocked) {
        $scope.userId = userId;
        $scope.isLocked = isLocked;
    }

    $scope.UpdateStatus = function () {
        $.ajax({
            url: "../../user/update-status",
            data: {
                userId: $scope.userId,
                isLocked: $scope.isLocked
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.GetList();

                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    }

    $scope.GetList = function () {
        $.ajax({
            url: "../../user/get-user-list",
            data: {
                userName: $scope.username,
                roleId: $scope.roleId,
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
                $scope.RspList = data.RspList;
                if (data.RspList.length > 0) {
                    $scope.totalRow = data.TotalRow;
                    //Init index for record
                    $scope.fromRecord = (($scope.currentPage - 1) * $scope.itemsPerPage) + 1;
                    $scope.toRecord = ($scope.currentPage * $scope.itemsPerPage);
                    if ($scope.toRecord > $scope.totalRow) {
                        $scope.toRecord = $scope.totalRow;
                    }

                    var count = -1;
                    for (var i = $scope.fromRecord; i <= $scope.toRecord; i++) {
                        count++;
                        data.RspList[count].index = i;
                    }
                } else {
                    $scope.totalRow = 0;
                }
                console.log($scope.RspList);
                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    };

    //===========Paging Bootstrap=====
    $scope.pageChanged = function () {
        $scope.GetList();
    };
}]);

vnpApp.controller('CategoryController', ["$scope", function ($scope) {
    $scope.ListData = [];
    $scope.RspCode = "";
    $scope.totalRow = 0;
    $scope.itemsPerPage = 10;
    $scope.currentPage = 1;
    $scope.maxSize = 6;

    $scope.module = "";
    $scope.title = "";
    $scope.isLang = "vi";
    $scope.isActive = "";
    //End init

    $scope.SetLang = function (lang) {
        $scope.isLang = lang;
        console.log(lang);
    }

    $scope.SetCate = function (id, status) {
        $scope.id = id;
        $scope.isActive = status;
    }



    $scope.UpdateStatus = function () {
        $.ajax({
            url: "../../category/update-status",
            data: {
                cateId: $scope.id,
                status: $scope.isActive
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.GetListCat();

                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    }

    $scope.GetListCat = function () {
        $.ajax({

            url: "../../category/get-cate-list",
            data: {
                CateName: $scope.CateName,
                CateStatus: $scope.CateStatus,
                pageNo: $scope.currentPage,
                table_list_length: $scope.table_list_length
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.ListData = data.ListData;
                if (typeof ($scope.table_list_length) != "undefined") {
                    $scope.itemsPerPage = $scope.table_list_length;
                }
                if (data.ListData.length > 0) {
                    $scope.totalRow = data.TotalRow;
                    //Init index for record
                    $scope.fromRecord = (($scope.currentPage - 1) * $scope.itemsPerPage) + 1;
                    $scope.toRecord = ($scope.currentPage * $scope.itemsPerPage);
                    if ($scope.toRecord > $scope.totalRow) {
                        $scope.toRecord = $scope.totalRow;
                    }

                    var count = -1;
                    for (var i = $scope.fromRecord; i <= $scope.toRecord; i++) {
                        count++;
                        data.ListData[count].index = i;

                    }
                } else {
                    $scope.totalRow = 0;
                }
                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    };

    //===========Paging Bootstrap=====
    $scope.pageChanged = function () {
        $scope.GetListCat();
    };

    $scope.SplitArr = function (input) {
        var arr = input.split(",");
        var rsArr = [];
        if (arr.length > 0) {
            for (var i = 0; i < arr.length; i++) {
                if (arr[i] !== null && arr[i] !== "") {
                    rsArr.push(arr[i]);
                }
            }
        }
        return rsArr;
    }
}]);

vnpApp.controller('PageController', ["$scope", function ($scope) {
    $scope.RspList = [];
    $scope.totalRow = 0;
    $scope.itemsPerPage = 12;
    $scope.currentPage = 1;
    $scope.maxSize = 6;
    //Init variable
    $scope.title = "";
    $scope.pageId = "0";
    $scope.status = "";
    $scope.PageTitle = "";
    $scope.PageAlias = "";
    //End init

    $scope.ChangeTitle = function () {
        $scope.PageAlias = slug($scope.PageTitle, "-");
    }

    $scope.SetPage = function (pageId, status) {
        $scope.pageId = pageId;
        $scope.status = status;
    }

    $scope.UpdateStatus = function () {
        $.ajax({
            url: "../../page/update-status",
            data: {
                pageId: $scope.pageId,
                status: $scope.status
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.GetList();

                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    }

    $scope.GetList = function () {
        $.ajax({
            url: "../../page/get-page-list",
            data: {
                title: $scope.title,
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
                $scope.RspList = data.RspList;
                console.log(data.RspList);
                if (data.RspList.length > 0) {
                    $scope.totalRow = data.TotalRow;
                    //Init index for record
                    $scope.fromRecord = (($scope.currentPage - 1) * $scope.itemsPerPage) + 1;
                    $scope.toRecord = ($scope.currentPage * $scope.itemsPerPage);
                    if ($scope.toRecord > $scope.totalRow) {
                        $scope.toRecord = $scope.totalRow;
                    }
                    var count = -1;
                    for (var i = $scope.fromRecord; i <= $scope.toRecord; i++) {
                        count++;
                        data.RspList[count].index = i;
                    }
                } else {
                    $scope.totalRow = 0;
                }
                console.log($scope.RspList);
                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    };

    //===========Paging Bootstrap=====
    $scope.pageChanged = function () {
        $scope.GetList();
    };
}]);
vnpApp.controller('SettingsController', ["$scope", function ($scope) {
    $scope.ListData = [];

    $scope.totalRow = 0;
    $scope.itemsPerPage = 10;
    $scope.currentPage = 1;
    $scope.maxSize = 6;
    $scope.title = "";
    //End init
    $scope.DeleteID = function (id) {
        $scope.id = id;
    }
    $scope.Delete = function () {
        $.ajax({
            url: "../../settings/delete",
            data: {
                id: $scope.id
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.GetListSettings();

                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    }
    $scope.GetListSettings = function () {
        $.ajax({

            url: "../../settings/get-settings",
            data: {
                SetCode: $scope.SetCode,
                SetKey: $scope.SetKey,
                SetStatus: $scope.SetStatus,
                pageNo: $scope.currentPage,
                tableListLength: $scope.table_list_length
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.ListData = data.ListData;
                if (typeof ($scope.table_list_length) != "undefined") {
                    $scope.itemsPerPage = $scope.table_list_length;
                }
                console.log(data.ListData);
                if (data.ListData.length > 0) {
                    $scope.totalRow = data.TotalRow;
                    //Init index for record
                    $scope.fromRecord = (($scope.currentPage - 1) * $scope.itemsPerPage) + 1;
                    $scope.toRecord = ($scope.currentPage * $scope.itemsPerPage);
                    if ($scope.toRecord > $scope.totalRow) {
                        $scope.toRecord = $scope.totalRow;
                    }

                    var count = -1;
                    for (var i = $scope.fromRecord; i <= $scope.toRecord; i++) {
                        count++;
                        data.ListData[count].index = i;
                    }
                } else {
                    $scope.totalRow = 0;
                }
                console.log($scope.ListData);
                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    };

    //===========Paging Bootstrap=====
    $scope.pageChanged = function () {
        $scope.GetListSettings();
    };
}]);
vnpApp.controller('BankController', ['$scope', '$http', function ($scope, $http) {
    $scope.ListData = [];
    $scope.totalRow = 0;
    $scope.itemsPerPage = 10;
    $scope.currentPage = 1;
    $scope.maxSize = 6;

    $scope.module = "";
    $scope.isActive = "";
    $scope.title = "";
    //End init
    $scope.SetCate = function (id, status) {
        $scope.id = id;
        $scope.isActive = status;
    }

    $scope.UpdateStatus = function () {
        $.ajax({
            url: "../../bank/update-status",
            data: {
                bankId: $scope.id,
                status: $scope.isActive
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.GetListBank();

                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    }

    $scope.GetListBank = function () {
        $.ajax({

            url: "../../bank/get-bank",
            data: {
                bankStatus: $scope.BankStatus,
                bankName: $scope.BankName,
                pageNo: $scope.currentPage,
                tableListLength: $scope.table_list_length
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.ListData = data.ListData;
                if (typeof ($scope.table_list_length) != "undefined") {
                    $scope.itemsPerPage = $scope.table_list_length;
                }
                console.log(data.ListData);
                if (data.ListData.length > 0) {
                    $scope.totalRow = data.TotalRow;
                    //Init index for record
                    $scope.fromRecord = (($scope.currentPage - 1) * $scope.itemsPerPage) + 1;
                    $scope.toRecord = ($scope.currentPage * $scope.itemsPerPage);
                    if ($scope.toRecord > $scope.totalRow) {
                        $scope.toRecord = $scope.totalRow;
                    }

                    var count = -1;
                    for (var i = $scope.fromRecord; i <= $scope.toRecord; i++) {
                        count++;
                        data.ListData[count].index = i;
                    }
                } else {
                    $scope.totalRow = 0;
                }
                console.log($scope.ListData);
                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    };

    //===========Paging Bootstrap=====
    $scope.pageChanged = function () {
        $scope.GetListBank();
    };
}]);
vnpApp.controller('MerchantController', ['$scope', '$http', function ($scope, $http) {
    $scope.ListData = [];
    $scope.totalRow = 0;
    $scope.itemsPerPage = 10;
    $scope.currentPage = 1;
    $scope.maxSize = 6;

    $scope.module = "";
    $scope.isActive = "";
    $scope.title = "";
    //End init
    $scope.SetCate = function (id, status) {
        $scope.id = id;
        $scope.isActive = status;
    }

    $scope.UpdateStatus = function () {
        $.ajax({
            url: "../../merchant/update-status",
            data: {
                merchantId: $scope.id,
                status: $scope.isActive
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.GetListMerchant();

                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    }

    $scope.GetListMerchant = function () {
        $.ajax({

            url: "../../merchant/get-merchant",
            data: {
                merchantStatus: $scope.merchantStatus,
                merchantName: $scope.merchantName,
                pageNo: $scope.currentPage,
                tableListLength: $scope.table_list_length
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.ListData = data.ListData;
                if (typeof ($scope.table_list_length) != "undefined") {
                    $scope.itemsPerPage = $scope.table_list_length;
                }
                console.log(data.ListData);
                if (data.ListData.length > 0) {
                    $scope.totalRow = data.TotalRow;
                    //Init index for record
                    $scope.fromRecord = (($scope.currentPage - 1) * $scope.itemsPerPage) + 1;
                    $scope.toRecord = ($scope.currentPage * $scope.itemsPerPage);
                    if ($scope.toRecord > $scope.totalRow) {
                        $scope.toRecord = $scope.totalRow;
                    }

                    var count = -1;
                    for (var i = $scope.fromRecord; i <= $scope.toRecord; i++) {
                        count++;
                        data.ListData[count].index = i;
                    }
                } else {
                    $scope.totalRow = 0;
                }
                console.log($scope.ListData);
                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    };

    //===========Paging Bootstrap=====
    $scope.pageChanged = function () {
        $scope.GetListMerchant();
    };
}]);
vnpApp.controller('TradeMarkController', ['$scope', '$http', function ($scope, $form) {
    $scope.ListData = [];
    $scope.ListData1 = [];
    $scope.totalRow = 0;
    $scope.itemsPerPage = 10;
    $scope.currentPage = 1;
    $scope.maxSize = 6;
    $scope.priority = [];
    $scope.nopriority = [];
    $scope.module = "";
    $scope.isActive = "";
    $scope.title = "";
    //End init
    $scope.SetCate = function (id, status) {
        $scope.id = id;
        $scope.isActive = status;
    }
    $scope.SetDelete = function (id) {
        $scope.id = id;
    }

    $scope.selected = {};
    $scope.GetMerchantChecked = function () {
        console.log($scope.selected);
        angular.forEach($scope.selected, function (value, key) {
            $scope.priority.push(key);
        });
        $.ajax({
            url: "../../trademark/update-priority",
            type: "POST",
            data: { tmpriority: $scope.priority },
            dataType: "json",
            traditional: true,
            success: function (response) {
                if (response.success) {
                    $scope.GetListPriotity();
                    $scope.GetListNoPriotity();
                } else {
                    alert(response.responseText);
                }
            },
            error: function (response) {
                alert("Bạn đang ở quyền ưu tiên"); //
            }
        });
        $scope.GetListPriotity();
        $scope.GetListNoPriotity();
    };
    $scope.selected1 = {};
    $scope.GetMerchantCheckedNo = function () {
        console.log($scope.selected1);
        angular.forEach($scope.selected1, function (value, key) {
            $scope.nopriority.push(key);
        });
        $.ajax({
            url: "../../trademark/update-nopriority",
            type: "POST",
            data: { tmnopriority:$scope.nopriority },
            dataType: "json",
            traditional: true,
            success: function (response) {
                if (response.success) {
                    $scope.GetListPriotity();
                    $scope.GetListNoPriotity();
                } else {
                    alert(response.responseText);
                }
            },
            error: function (response) {
                alert("Bạn đang ở quyền không ưu tiên"); //
            }
        });
       
    };

    $scope.UpdateStatus = function () {
        $.ajax({
            url: "../../trademark/update-status",
            data: {
                tmId: $scope.id,
                status: $scope.isActive
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.GetListTradeMark();

                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    }
    $scope.Delete = function () {
        $.ajax({
            type: "POST",
            url: '../../trademark/delete',
            dataType: "json",
            data: { idt: $scope.id },
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (states) {
                if (states.success) {
                    $('#delete-popup').modal('hide');
                    $('#delete-popup-note').modal('show');
                    $scope.GetListTradeMark();
                } else {
                    $('#delete-popup').modal('hide');
                    $scope.GetListTradeMark();
                }
            },
            error: function (ex) {
                alert("Failed to retrieve states." + ex);
            }
        });
        return false;
    }
    $scope.DeleteAll = function () {
        var i = $scope.id;
        alert(i);
        $.ajax({
            type: "POST",
            url: '../../trademark/deleteall',
            dataType: "json",
            data: { idt: $scope.id },
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (states) {
                if (states.success) {
                    $('#delete-popup-note').modal('hide');
                    $scope.GetListTradeMark();
                }
            },
            error: function (ex) {
                alert("Failed to retrieve states." + ex);
            }
        });
        return false;
    }
    $scope.GetListPriotity = function () {
        $.ajax({
            url: "../../trademark/get-Priotity",
            data: {

            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.ListData = data.ListData;
                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    };
    $scope.GetListNoPriotity = function () {
        $.ajax({
            url: "../../trademark/get-NoPriotity",
            data: {

            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.ListData1 = data.ListData1;
                var count = -1;
                for (var i = 1; i <= $scope.ListData1.length; i++) {
                    count++;
                    data.ListData1[count].index = i;
                }
                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    };

    $scope.GetListTradeMark = function () {
        $.ajax({
            url: "../../trademark/get-trademark",
            data: {
                TmName: $scope.TmName,
                Status: $scope.Status,
                pageNo: $scope.currentPage,
                tableListLength: $scope.table_list_length
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.ListData = data.ListData;
                if (typeof ($scope.table_list_length) != "undefined") {
                    $scope.itemsPerPage = $scope.table_list_length;
                }
                console.log(data.ListData);
                if (data.ListData.length > 0) {
                    $scope.totalRow = data.TotalRow;
                    //Init index for record
                    $scope.fromRecord = (($scope.currentPage - 1) * $scope.itemsPerPage) + 1;
                    $scope.toRecord = ($scope.currentPage * $scope.itemsPerPage);
                    if ($scope.toRecord > $scope.totalRow) {
                        $scope.toRecord = $scope.totalRow;
                    }

                    var count = -1;
                    for (var i = $scope.fromRecord; i <= $scope.toRecord; i++) {
                        count++;
                        data.ListData[count].index = i;
                    }
                } else {
                    $scope.totalRow = 0;
                }
                console.log($scope.ListData);
                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    };

    //===========Paging Bootstrap=====
    $scope.pageChanged = function () {
        $scope.GetListTradeMark();
    };
}]);

vnpApp.controller('PostController', ["$scope", "$filter", function ($scope, $filter) {
    $scope.RspList = [];
    $scope.totalRow = 0;
    $scope.itemsPerPage = 10;
    $scope.currentPage = 1;
    $scope.maxSize = 6;
    $scope.txtFromDate = "";
    $scope.txtToDate = "";
    $scope.postStatus = "";
    $scope.postId = "";
    $scope.isLang = "vi";
    //End init

    $scope.SetStatus = function (postId, poststatus) {
        $scope.postId = postId;
        $scope.postStatus = poststatus;
    }

    $scope.UpdateStatus = function () {
        $.ajax({
            url: "../../post/update-status",
            data: {
                postId: $scope.postId,
                postStatus: $scope.postStatus
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.GetList();

                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    }

    $scope.Delete = function () {
        $.ajax({
            url: "../../post/delete",
            data: {
                originId: $scope.originId
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.GetList();

                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    }

  
    $scope.GetList = function () {
        var txtFromDate = $('#txtFromDate').val();
        var txtToDate = $('#txtToDate').val();
        $.ajax({
            url: "../../post/get-post-list",
            data: {
                PostTitle: $scope.PostTitle,
                PostStatus: $scope.PostStatus,
                txtFromDate: txtFromDate,
                txtToDate: txtToDate,
                pageNo: $scope.currentPage,
                tableListLength: $scope.table_list_length
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.RspList = data.RspList;
                if (typeof ($scope.table_list_length) != "undefined") {
                    $scope.itemsPerPage = $scope.table_list_length;
                }
                if (data.RspList.length > 0) {
                    $scope.totalRow = data.TotalRow;
                    //Init index for record
                    $scope.fromRecord = (($scope.currentPage - 1) * $scope.itemsPerPage) + 1;
                    $scope.toRecord = ($scope.currentPage * $scope.itemsPerPage);
                    if ($scope.toRecord > $scope.totalRow) {
                        $scope.toRecord = $scope.totalRow;
                    }

                    var count = -1;
                    for (var i = $scope.fromRecord; i <= $scope.toRecord; i++) {
                        count++;
                        data.RspList[count].index = i;
                    }
                } else {
                    $scope.totalRow = 0;
                }
                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    };


    //===========Paging Bootstrap=====
    $scope.pageChanged = function () {
        $scope.GetList();
    };

}]);
// Receive News
vnpApp.controller('ReceiveNewsController', ["$scope", function ($scope, $window) {
    $scope.ListData = [];
    $scope.RspCode = "";
    $scope.totalRow = 0;
    $scope.itemsPerPage = 10;
    $scope.currentPage = 1;
    $scope.maxSize = 6;

    $scope.module = "";

    $scope.title = "";
    $scope.isLang = "vi";
    $scope.isActive = "";
    //End init

    $scope.SetLang = function (lang) {
        $scope.isLang = lang;
        console.log(lang);
    }
    $scope.GetListReceiveNews = function () {
        $.ajax({

            url: "../../receivenews/get-receivenews",
            data: {
                RnName: $scope.RnName,
                pageNo: $scope.currentPage,
                table_list_length: $scope.table_list_length
            },
            type: "POST",
            beforeSend: function () {
                $(".loading").show();
            },
            complete: function () {
                $(".loading").hide();
            },
            success: function (data) {
                $scope.ListData = data.ListData;
                if (typeof ($scope.table_list_length) != "undefined") {
                    $scope.itemsPerPage = $scope.table_list_length;
                }
                if (data.ListData.length > 0) {
                    $scope.totalRow = data.TotalRow;
                    //Init index for record
                    $scope.fromRecord = (($scope.currentPage - 1) * $scope.itemsPerPage) + 1;
                    $scope.toRecord = ($scope.currentPage * $scope.itemsPerPage);
                    if ($scope.toRecord > $scope.totalRow) {
                        $scope.toRecord = $scope.totalRow;
                    }

                    var count = -1;
                    for (var i = $scope.fromRecord; i <= $scope.toRecord; i++) {
                        count++;
                        data.ListData[count].index = i;

                    }
                } else {
                    $scope.totalRow = 0;
                }
                if (!$scope.$$phase)
                    $scope.$apply();
            },
            error: ""
        });
    };
    //===========Paging Bootstrap=====
    $scope.pageChanged = function () {
        $scope.GetListReceiveNews();
    };

    
}]);
