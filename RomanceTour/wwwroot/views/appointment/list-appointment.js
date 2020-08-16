function Initialize() {
	var header = parseFloat($("header").height());
	var collapse = $(".collapse").css("display") == "none" || $(window).width() >= 1200 ? 0 : parseFloat($(".collapse").height() + parseFloat($(".collapse").css("paddingTop")) * 2);
	var padding = parseFloat($(".full-screen-area").css("paddingTop"));

	$("article").height(window.innerHeight - header + collapse - (padding * 2));
	$(window).resize(function () {
		var header = parseFloat($("header").height());
		var collapse = $(".collapse").css("display") == "none" || $(window).width() >= 1200 ? 0 : parseFloat($(".collapse").height()) + (parseFloat($(".collapse").css("paddingTop")) * 2);

		var padding = parseFloat($(".full-screen-area").css("paddingTop"));
		$("article").height(window.innerHeight - header + collapse - (padding * 2));
	});
}

$(document).ready(function () {
	Initialize();

	$(".appointment-item").on("click", function () {
		var id = parseInt($(this).children(".appointment-id").val());
		var isUserAppointment = $(this).children(".is-user-appointment").val() == "True" ? true : false;
		if (isUserAppointment) window.location.href = `/Appointment/GetAppointment?id=${id}`;
		else window.location.href = `/Appointment/TryGetAppointment?id=${id}`;
	});
});