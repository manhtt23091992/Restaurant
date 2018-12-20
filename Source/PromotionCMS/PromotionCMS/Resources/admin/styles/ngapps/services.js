vnpApp.service("VnpayService", function ($http) {
    //Read all Students
    this.getPages = function () {
        return $http.get("/api/PageApi");
    };

});
