var app = angular.module('CpoBackend', ['ngSanitize']);
app.config(['$httpProvider', function ($httpProvider) {
	delete $httpProvider.defaults.headers.common['X-Requested-With'];
}]);
//app.config(['$mdDateLocaleProvider', function ($mdDateLocaleProvider) {
//	moment.locale('de');
//	$mdDateLocaleProvider.formatDate = function (date) {
//		return moment(date).format('L');
//	};
//	$mdDateLocaleProvider.parseDate = function (dateString) {
//		var m = moment(dateString, 'L', true);
//		return m.isValid() ? m.toDate() : new Date(NaN);
//	};
//	$mdDateLocaleProvider.firstDayOfWeek = 1;
//}]);
//app.config(['$ariaProvider', function config($ariaProvider) {
//	$ariaProvider.config({
//		ariaValue: false,
//		bindRoleForClick: false
//	});
//}]);
app.controller('StickerController', ['$scope', '$http', '$interval', '$window', '$filter', '$timeout', '$q', function ($scope, $http, $interval, $window, $filter, $timeout, $q) {
	var $s = $scope;
	$s.queryStringParams = new URLSearchParams($window.location.search);





	$s.getChargePoint = function () {
		$http.get("gatekeeper/CrmSyncProject?ProjectGuid=" + project.guid).then(function success(response) {
			$s.activeproject = null;
			$s.selectProject(project);
		}, onError);

	}
	$s.ChargePoints = [];

	const gpsFormat = new Intl.NumberFormat('de-DE',
		{
			maximumSignificantDigits: 3,
			maximumSignificantDigits: 3
		});

	$http.get('api/Sticker/Chargepoints').then((response) => {
		$s.ChargePoints = Enumerable.From(response.data).Where(function (c) { return ((c.State & 192) != 192); }).OrderBy(function (c) { return c.ZipCode; }).ToArray();
		//$s.ChargePoints = Enumerable.From(response.data).Where(function (c) { return ((c.State & 32) > 0) && ((c.State & 224) !=224); }).ToArray();
	});

	$s.shorten = function (dVal) {
		return dVal;
		return gpsFormat.format(dVal);
	}
	$s.toggleState = function (chargePoint, state) {
		chargePoint.State ^= state;
		$http.get('api/Sticker/Update?cpguid=' + chargePoint.Guid + '&userguid=' + chargePoint.PartitionKey + '&cpstate=' + chargePoint.State).then((response) => {
			//$.ChargePoints = response.data;
		});
	}
	$s.hasState = function (chargePoint, state) {
		return (chargePoint.State & state)>0;
	}


	function onError(response) {
	};

}]);
