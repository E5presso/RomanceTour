function ValidateName(name) {
	var regex = /^[가-힣]{2,4}$/;
	return regex.test(name);
}
function ValidatePhone(phone) {
	var regex = /(01[016789])([1-9]{1}[0-9]{2,3})([0-9]{4})$/;
	return regex.test(phone);
}
function ValidateForm() {
	setTimeout(function () {
		if ($(".name").hasClass("is-valid") &&
			$(".phone").hasClass("is-valid"))
			$(".apply-btn").prop("disabled", false);
		else $(".apply-btn").prop("disabled", true);
	}, 10);
}

function Initialize() {
	var header = parseFloat($("header").height()) + parseFloat($("header").css("paddingTop")) * 2;
	var collapse = $(".collapse").css("display") == "none" || $(window).width() >= 1200 ? 0 : parseFloat($(".collapse").height() + parseFloat($(".collapse").css("paddingTop")) * 2);
	var padding = parseFloat($(".full-screen-area").css("paddingTop"));

	$("article").height(window.innerHeight - header + collapse - (padding * 2));
	$(window).resize(function () {
		var header = parseFloat($("header").height()) + parseFloat($("header").css("paddingTop")) * 2;
		var collapse = $(".collapse").css("display") == "none" || $(window).width() >= 1200 ? 0 : parseFloat($(".collapse").height() + parseFloat($(".collapse").css("paddingTop")) * 2);
		var padding = parseFloat($(".full-screen-area").css("paddingTop"));

		$("article").height(window.innerHeight - header + collapse - (padding * 2));
	});
}

$(document).ready(function () {
	Initialize();

	$(".name").on("keyup", function () {
		var value = $(".name").val();
		if (value.length > 0)
		{
			if (ValidateName(value))
			{
				$(".name").siblings("label").html("유효한 이름입니다.");
				$(".name").siblings("label").removeClass("text-danger");
				$(".name").siblings("label").addClass("text-success");
				$(".name").removeClass("is-invalid");
				$(".name").addClass("is-valid");
			}
			else
			{
				$(".name").siblings("label").html("유효하지 않은 이름입니다.");
				$(".name").siblings("label").removeClass("text-success");
				$(".name").siblings("label").addClass("text-danger");
				$(".name").removeClass("is-valid");
				$(".name").addClass("is-invalid");
			}
		}
		else
		{
			$(".name").removeClass("is-invalid");
			$(".name").removeClass("is-valid");
			$(".name").siblings("label").html("이름");
			$(".name").siblings("label").removeClass("text-danger");
			$(".name").siblings("label").removeClass("text-success");
		}
		ValidateForm();
	});
	$(".phone").on("keyup", function () {
		var value = $(".phone").val();
		if (value.length > 0)
		{
			if (ValidatePhone(value))
			{
				$(".phone").siblings("label").html("유효한 번호입니다.");
				$(".phone").siblings("label").removeClass("text-danger");
				$(".phone").siblings("label").addClass("text-success");
				$(".phone").removeClass("is-invalid");
				$(".phone").addClass("is-valid");
			}
			else
			{
				$(".phone").siblings("label").html("유효하지 않은 번호입니다.");
				$(".phone").siblings("label").removeClass("text-success");
				$(".phone").siblings("label").addClass("text-danger");
				$(".phone").removeClass("is-valid");
				$(".phone").addClass("is-invalid");
			}
		}
		else
		{
			$(".phone").removeClass("is-invalid");
			$(".phone").removeClass("is-valid");
			$(".phone").siblings("label").html("휴대폰 번호");
			$(".phone").siblings("label").removeClass("text-danger");
			$(".phone").siblings("label").removeClass("text-success");
		}
		ValidateForm();
	});
	$(".apply-btn").on("click", function (e) {
		e.preventDefault();

		var name = $(".name").val();
		var phone = $(".phone").val();
		window.location.href = `/Appointment/LookupAppointment?name=${name}&phone=${phone}`;
	});
	$(".login-btn").on("click", function (e) {
		e.preventDefault();
		window.location.href = "/User/Login";
	});

	$(".name").focus();
});