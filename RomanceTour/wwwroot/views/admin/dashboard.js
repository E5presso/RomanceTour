function Initialize() {
	var header = parseFloat($("header").height()) + parseFloat($("header").css("paddingTop")) * 2;
	var collapse = $(".collapse").css("display") == "none" || $(window).width() >= 1200 ? 0 : parseFloat($(".collapse").height() + parseFloat($(".collapse").css("paddingTop")) * 2);
	var padding = parseFloat($(".full-screen-area").css("paddingTop"));

	$("article").height(window.innerHeight - header + collapse - (padding * 2));
	if ($(window).width() > 576 && IsMobile)
	{
		$(".third .btn-text").css("display", "inline");
		$(".third .btn-text").css("opacity", "1");
	}
	else
	{
		$(".third .btn-text").css("display", "none");
		$(".third .btn-text").css("opacity", "0");
	}
	$(window).resize(function () {
		var header = parseFloat($("header").height()) + parseFloat($("header").css("paddingTop")) * 2;
		var collapse = $(".collapse").css("display") == "none" || $(window).width() >= 1200 ? 0 : parseFloat($(".collapse").height() + parseFloat($(".collapse").css("paddingTop")) * 2);
		var padding = parseFloat($(".full-screen-area").css("paddingTop"));

		$("article").height(window.innerHeight - header + collapse - (padding * 2));
		if ($(window).width() > 576 && IsMobile)
		{
			$(".third .btn-text").css("display", "inline");
			$(".third .btn-text").css("opacity", "1");
		}
		else
		{
			$(".third .btn-text").css("display", "none");
			$(".third .btn-text").css("opacity", "0");
		}
	});
}

$(document).ready(function () {
	$(".third").hover(
		function () {
			if ($(window).width() > 576 && !IsMobile)
			{
				$(this).find(".btn-text").stop().slideDown(DURATION);
				$(this).find(".btn-text").css("opacity", "1");
			}
		},
		function () {
			if ($(window).width() > 576 && !IsMobile)
			{
				$(this).find(".btn-text").stop().slideUp(DURATION);
				$(this).find(".btn-text").css("opacity", "1");
			}
		}
	);
	$(".third").on("click", function () {
		if (!IsMobile) $(this).find(".btn-text").stop().slideUp(DURATION);
	});
	$(".manage-user").on("click", function () {
		window.location.href = "/Admin/ManageUser";
	});
	$(".manage-product").on("click", function () {
		window.location.href = "/Admin/ManageProduct";
	});
	$(".manage-appointment").on("click", function () {
		window.location.href = "/Admin/ManageAppointment";
	});
	$(".manage-etc").on("click", function () {
		window.location.href = "/Admin/ManageEtc";
	});

	Initialize();
});