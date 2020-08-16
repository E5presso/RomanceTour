var IsMobile = navigator.maxTouchPoints > 1;
var DELAY = 300;
var ResultType =
{
	SUCCESS: "SUCCESS",
	SYSTEM_ERROR: "SYSTEM_ERROR",
	LOGIN_REQUIRED: "LOGIN_REQUIRED",
	ACCESS_DENIED: "ACCESS_DENIED",
	NOT_FOUND: "NOT_FOUND",
	INVALID_ACCESS: "INVALID_ACCESS",
	ALREADY_EXISTS: "ALREADY_EXISTS"
}
var Orientation =
{
	Vertical: "VERTICAL",
	Horizontal: "HORIZONTAL"
}
const Minute = 60000;

function Ajax(url, data, callback) {
	ShowLoading();
	$.ajax({
		url: url,
		type: "POST",
		data: data,
		success: function (response) {
			switch (response.Result)
			{
				case ResultType.SUCCESS: {
					callback(response.Model);
					HideLoading();
					break;
				}
				case ResultType.NOT_FOUND: {
					alert("해당 요청을 찾을 수 없습니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.ALREADY_EXISTS: {
					alert("이미 처리된 요청입니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.LOGIN_REQUIRED: {
					alert("로그인이 필요한 서비스입니다.");
					window.location.href = "/User/Login";
					break;
				}
				case ResultType.ACCESS_DENIED: {
					alert("접근이 거부되었습니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.INVALID_ACCESS: {
					alert("잘못된 요청입니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.SYSTEM_ERROR: {
					window.location.href = "/Home/Error";
					break;
				}
			}
		}
	});
}
function AjaxWithoutLoading(url, data, callback) {
	$.ajax({
		url: url,
		type: "POST",
		data: data,
		success: function (response) {
			switch (response.Result)
			{
				case ResultType.SUCCESS: {
					callback(response.Model);
					break;
				}
				case ResultType.NOT_FOUND: {
					alert("해당 요청을 찾을 수 없습니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.ALREADY_EXISTS: {
					alert("이미 처리된 요청입니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.LOGIN_REQUIRED: {
					alert("로그인이 필요한 서비스입니다.");
					window.location.href = "/User/Login";
					break;
				}
				case ResultType.ACCESS_DENIED: {
					alert("접근이 거부되었습니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.INVALID_ACCESS: {
					alert("잘못된 요청입니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.SYSTEM_ERROR: {
					window.location.href = "/Home/Error";
					break;
				}
			}
		}
	});
}
function AjaxForm(url, data, callback) {
	ShowLoading();
	$.ajax({
		url: url,
		type: "POST",
		contentType: false,
		processData: false,
		data: data,
		success: function (response) {
			switch (response.Result)
			{
				case ResultType.SUCCESS: {
					callback(response.Model);
					HideLoading();
					break;
				}
				case ResultType.NOT_FOUND: {
					alert("해당 요청을 찾을 수 없습니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.ALREADY_EXISTS: {
					alert("이미 처리된 요청입니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.LOGIN_REQUIRED: {
					alert("로그인이 필요한 서비스입니다.");
					window.location.href = "/User/Login";
					break;
				}
				case ResultType.ACCESS_DENIED: {
					alert("접근이 거부되었습니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.INVALID_ACCESS: {
					alert("잘못된 요청입니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.SYSTEM_ERROR: {
					window.location.href = "/Home/Error";
					break;
				}
			}
		}
	});
}
function AjaxFormWithoutLoading(url, data, callback) {
	$.ajax({
		url: url,
		type: "POST",
		contentType: false,
		processData: false,
		data: data,
		success: function (response) {
			switch (response.Result)
			{
				case ResultType.SUCCESS: {
					callback(response.Model);
					break;
				}
				case ResultType.NOT_FOUND: {
					alert("해당 요청을 찾을 수 없습니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.ALREADY_EXISTS: {
					alert("이미 처리된 요청입니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.LOGIN_REQUIRED: {
					alert("로그인이 필요한 서비스입니다.");
					window.location.href = "/User/Login";
					break;
				}
				case ResultType.ACCESS_DENIED: {
					alert("접근이 거부되었습니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.INVALID_ACCESS: {
					alert("잘못된 요청입니다.");
					window.location.href = "/Home/Index";
					break;
				}
				case ResultType.SYSTEM_ERROR: {
					window.location.href = "/Home/Error";
					break;
				}
			}
		}
	});
}

function padding(n, width) {
	n = n + '';
	return n.length >= width ? n : new Array(width - n.length + 1).join('0') + n;
}
jQuery.fn.hasScrollBar = function (direction) {
	if (direction == Orientation.Vertical)
		return this.get(0).scrollHeight > this.innerHeight();
	else if (direction == Orientation.Horizontal)
		return this.get(0).scrollWidth > this.innerWidth();
	else return false;
}

Number.prototype.format = function () {
	if (this == 0) return 0;
	var reg = /(^[+-]?\d+)(\d{3})/;
	var n = (this + '');
	while (reg.test(n)) n = n.replace(reg, '$1' + ',' + '$2');
	return n;
};
Number.prototype.zf = function (len) { return this.toString().zf(len); };

Date.prototype.toDotNetDateTime = function () {
	var day = this.getDate();
	var month = this.getMonth() + 1;
	var year = this.getFullYear();
	var hour = this.getHours();
	var minute = this.getMinutes();
	var second = this.getSeconds();

	return year + "/" + padding(month, 2) + "/" + padding(day, 2) + " " + padding(hour, 2) + ':' + padding(minute, 2) + ':' + padding(second, 2);
}
Date.prototype.toDateInputValue = (function () {
	var local = new Date(this);
	local.setMinutes(this.getMinutes() - this.getTimezoneOffset());
	return local.toJSON().slice(0, 10);
});
Date.prototype.format = function (f) {
	if (!this.valueOf()) return " ";

	var weekName = ["일요일", "월요일", "화요일", "수요일", "목요일", "금요일", "토요일"];
	var d = this;

	return f.replace(/(yyyy|yy|MM|dd|E|hh|mm|ss|a\/p)/gi, function ($1) {
		switch ($1)
		{
			case "yyyy": return d.getFullYear();
			case "yy": return (d.getFullYear() % 1000).zf(2);
			case "MM": return (d.getMonth() + 1).zf(2);
			case "dd": return d.getDate().zf(2);
			case "E": return weekName[d.getDay()];
			case "HH": return d.getHours().zf(2);
			case "hh": return ((h = d.getHours() % 12) ? h : 12).zf(2);
			case "mm": return d.getMinutes().zf(2);
			case "ss": return d.getSeconds().zf(2);
			case "a/p": return d.getHours() < 12 ? "오전" : "오후";
			default: return $1;
		}
	});
};

String.prototype.string = function (len) { var s = '', i = 0; while (i++ < len) { s += this; } return s; };
String.prototype.zf = function (len) { return "0".string(len - this.length) + this; };
String.prototype.replaceAll = function (org, dest) {
	return this.split(org).join(dest);
}

setInterval(function () {
	AjaxWithoutLoading("/Home/ExtendSessionTime", {}, function (model) {
		if (model) console.log("로그인 시간이 연장되었습니다.");
		else console.log("로그인 시간 연장에 실패했습니다.");
	});
}, 10 * Minute);