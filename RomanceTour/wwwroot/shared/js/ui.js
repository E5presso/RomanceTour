var DURATION = 300;

function InitContainer() {
	var header = parseFloat($("header").height()) + parseFloat($("header").css("paddingTop")) * 2;
	var collapse = $(".collapse").css("display") == "none" || $(".top-menu").css("display") != "none" ? 0 : parseFloat($(".collapse").height());
	var padding = parseFloat($("#loading").css("paddingTop"));

	$("article").css("top", header - collapse);
	$("#loading").css("top", header - collapse);
	$("#loading").height(window.innerHeight - header + collapse - (padding * 2));
	$(window).resize(function () {
		var header = parseFloat($("header").height()) + parseFloat($("header").css("paddingTop")) * 2;
		var collapse = $(".collapse").css("display") == "none" || $(".top-menu").css("display") != "none" ? 0 : parseFloat($(".collapse").height() + 10);
		var padding = parseFloat($("#loading").css("paddingTop"));

		$("article").css("top", header - collapse);
		$("#loading").css("top", header - collapse);
		$("#loading").height(window.innerHeight - header + collapse - (padding * 2));
	});
	var header = parseFloat($("header").height());
	var collapse = $(".collapse").css("display") == "none" || $(window).width() >= 1200 ? 0 : parseFloat($(".collapse").height());
	var scroll = $(window).scrollTop();
	if (header - collapse <= scroll)
	{
		if ($(".collapse").css("display") == "none" || $(window).width() >= 1200)
		{
			$(".top-menu").css("background", "rgba(1, 15, 59, 0.8)");
			$(".gnb").css("background", "rgba(2, 56, 89, 0.5)");
		}
		else
		{
			$(".top-menu").css("background", "rgba(1, 15, 59, 1)");
			$(".gnb").css("background", "rgba(2, 56, 89, 1)");
		}
	}
	else
	{
		$(".top-menu").css("background", "rgba(1, 15, 59, 1)");
		$(".gnb").css("background", "rgba(2, 56, 89, 1)");
	}
}
function ShowLoading() {
	$("#loading").fadeIn(DURATION);
	$("#loading").css("display", "flex");
}
function HideLoading() {
	$("#loading").fadeOut(DURATION);
}

function LogoutCallback(model) {
	HideLoading();
	if (model)
	{
		alert("로그아웃되었습니다.");
		window.location.reload();
	}
}

$(document).ready(function () {
	InitContainer();
	$(".search-text").on("focus", function () {
		$(this).parent().css("box-shadow", "0px 0px 10px #F2B705");
		$(this).parent().find(".search-btn > .fa-search").css({
			"color": "#F2B705",
			"text-shadow": "0px 0px 10px #F2B705"
		});
	});
	$(".search-text.has-shadow").on("focusout", function () {
		$(this).parent().css("box-shadow", "2px 2px 5px rgba(18, 18, 18, 0.3)");
		$(this).parent().find(".search-btn > .fa-search").css({
			"color": "#023859",
			"text-shadow": "none"
		});
	});
	$(".search-text:not(.has-shadow)").on("focusout", function () {
		$(this).parent().css("box-shadow", "initial");
		$(this).parent().find(".search-btn > .fa-search").css({
			"color": "#023859",
			"text-shadow": "none"
		});
	});
	$(".search-text").on("keyup", function (e) {
		if (e.keyCode == 13)
		{
			var keyword = $(this).val();
			$(this).val("");
			window.location.href = `/Product/ListProduct?category=0&keyword=${keyword}`;
		}
	});
	$(".search-btn").on("click", function () {
		var keyword = $(".search-text").val();
		$(".search-text").val("");
		window.location.href = `/Product/ListProduct?category=0&keyword=${keyword}`;
	});
	$(".navbar-toggler").on("click", function () {
		var header = parseFloat($("header").height());
		var collapse = $(".collapse").css("display") == "none" ? 0 : parseFloat($(".collapse").height() + parseFloat($(".collapse").css("paddingTop")) * 2);
		var scroll = $(window).scrollTop();

		if (header - collapse <= scroll)
		{
			if ($(".collapse").css("display") == "none" || $(window).width() >= 1200)
			{
				$(".top-menu").css("background", "rgba(1, 15, 59, 1)");
				$(".gnb").css("background", "rgba(2, 56, 89, 1)");
			}
			else
			{
				$(".top-menu").css("background", "rgba(1, 15, 59, 0.8)");
				$(".gnb").css("background", "rgba(2, 56, 89, 0.5)");
			}
		}
		else
		{
			$(".top-menu").css("background", "rgba(1, 15, 59, 1)");
			$(".gnb").css("background", "rgba(2, 56, 89, 1)");
		}
	});
	$(window).on("touchmove mousewheel DOMMouseScroll scroll", function () {
		var header = parseFloat($("header").height());
		var collapse = $(".collapse").css("display") == "none" ? 0 : parseFloat($(".collapse").height() + parseFloat($(".collapse").css("paddingTop")) * 2);
		var scroll = $(window).scrollTop();
		if (header - collapse <= scroll)
		{
			if ($(".collapse").css("display") == "none" || $(window).width() >= 1200)
			{
				$(".top-menu").css("background", "rgba(1, 15, 59, 0.8)");
				$(".gnb").css("background", "rgba(2, 56, 89, 0.5)");
			}
			else
			{
				$(".top-menu").css("background", "rgba(1, 15, 59, 1)");
				$(".gnb").css("background", "rgba(2, 56, 89, 1)");
			}
		}
		else
		{
			$(".top-menu").css("background", "rgba(1, 15, 59, 1)");
			$(".gnb").css("background", "rgba(2, 56, 89, 1)");
		}
	});
	$("header").on("touchmove mousewheel DOMMouseScroll scroll", function (e) {
		e.preventDefault();
		e.stopPropagation();
	});

	$(".login").on("click", function () {
		window.location.href = "/User/Login";
	});
	$(".register").on("click", function () {
		window.location.href = "/User/Register";
	});
	$(".logout").on("click", function () {
		Ajax("/User/Logout", {}, LogoutCallback);
	});
	$(".appointment").on("click", function () {
		window.location.href = "/Appointment/ListAppointment";
	});
	$(".mypage").on("click", function () {
		window.location.href = "/User/Mypage";
	});
	$(".dashboard").on("click", function () {
		window.location.href = "/Admin/Dashboard";
	});

	$("img").attr("draggable", false);
	$("img").on("dragstart", function (e) { e.preventDefault(); });
});