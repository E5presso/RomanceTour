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

	$(".password").on("keyup", function () {
		var value = $(".password").val();
		if (value.length > 0) $(".apply-btn").prop("disabled", false);
		else $(".apply-btn").prop("disabled", true);
	});
	$(".apply-btn").on("click", function (e) {
		e.preventDefault();

		var id = $("#appointment-id").val();
		var password = $(".password").val();

		var form = $(`
			<form>
				<input type="hidden" name="id" value="${id}" />
				<input type="hidden" name="password" value="${password}" />
			</form>
		`, {
			action: "/Appointment/TryGetAppointment",
			method: "POST",
			style: "display: none;"
		});
		form.appendTo("body").submit();
	});
	$(".password").focus();
});