var VerificationResult = {
	SUCCESS: "SUCCESS",							// 발송 성공 시
	FAILURE: "FAILURE",							// 발송 실패 시
	TOO_MUCH_REQUEST: "TOO_MUCH_REQUEST",		// 너무 빠른 요청
	MAX_REQUEST_REACHED: "MAX_REQUEST_REACHED",	// 일일 요청한도 도달
	USER_ALREADY_EXISTS: "USER_ALREADY_EXISTS",	// 이미 존재하는 사용자
	USER_NOT_FOUND: "USER_NOT_FOUND"			// 존재하지 않는 사용자
};
var token = "";
var timeLimit = 3 * 60;
var timer;

function ValidatePhone(phone)
{
	var regex = /(01[016789])([1-9]{1}[0-9]{2,3})([0-9]{4})$/;
	return regex.test(phone);
}
function ValidatePassword(passwd)
{
	var regex = /^(?=.*[a-zA-Z])(?=.*[0-9]).{4,20}$/;
	return regex.test(passwd);
}
function ValidateForm()
{
	setTimeout(function ()
	{
		if ($("#phone").hasClass("is-valid") &&
			$("#new-password").hasClass("is-valid") &&
			$("#confirm-password").hasClass("is-valid"))
			$("#change-password").prop("disabled", false);
		else $("#change-password").prop("disabled", true);
	}, 10);
}

function VerifyPhoneCallback(model)
{
	$("#phone").prop("disabled", false);
	switch (model)
	{
		case VerificationResult.SUCCESS: {
			timeLimit = 3 * 60;
			clearInterval(timer);
			timer = setInterval(function ()
			{
				if (timeLimit > 0)
				{
					timeLimit--;
					$("#validation-code").children("label").text(`인증번호 [${Math.floor(timeLimit / 60).zf(2)}:${(timeLimit % 60).zf(2)}]`);
				}
				else
				{
					clearInterval(timer);
					$("#validation-code").children("label").removeClass("text-danger");
					$("#code").removeClass("is-invalid");
					$("#code").val("");
					$("#challenge").prop("disabled", true);
					$("#validation-code").slideUp(DURATION);
					$("#phone").focus();
				}
			}, 1000);
			$("#validation-code").children("label").removeClass("text-danger");
			$("#code").removeClass("is-invalid");
			$("#code").val("");
			$("#challenge").prop("disabled", true);
			alert("인증요청 문자를 발송했습니다.");
			$("#validation-code").slideDown(DURATION);
			$("#code").focus();
			break;
		}
		case VerificationResult.FAILURE: {
			window.location.href = "/Home/Error";
			break;
		}
		case VerificationResult.TOO_MUCH_REQUEST: {
			alert("너무 빠른 인증요청입니다.\n잠시 후에 다시 시도해주세요.");
			break;
		}
		case VerificationResult.MAX_REQUEST_REACHED: {
			alert("일일 인증요청한도에 도달했습니다.");
			break;
		}
		case VerificationResult.USER_ALREADY_EXISTS: {
			alert("이미 사용 중인 번호입니다.");
			break;
		}
		case VerificationResult.USER_NOT_FOUND: {
			alert("등록되지 않은 번호입니다.");
			break;
		}
	}
}
function ChallengeCallback(model)
{
	$("#code").prop("disabled", false);
	if (model.Result)
	{
		clearInterval(timer);
		token = model.Token;
		alert("휴대폰 인증에 성공하였습니다.");
		$("#phone").prop("disabled", true);
		$("#validate-phone").prop("disabled", true);
		$("#validation-code").slideUp(DURATION);
		$("#phone").parent().siblings("label").html("인증된 번호입니다.");
		$("#phone").parent().siblings("label").removeClass("text-danger");
		$("#phone").parent().siblings("label").addClass("text-success");
		$("#phone").removeClass("is-invalid");
		$("#phone").addClass("is-valid");
		$("#password-area").slideDown();
		$("#confirm-password-area").slideDown();
		$("#new-password").focus();
	}
	else
	{
		alert("잘못된 인증번호입니다.");
		$("#code").parent().siblings("label").html("잘못된 인증번호입니다.");
		$("#code").parent().siblings("label").removeClass("text-success");
		$("#code").parent().siblings("label").addClass("text-danger");
		$("#code").addClass("is-invalid");
		$("#code").removeClass("is-valid");
		$("#validate-phone").prop("disabled", false);
		$("#code").focus();
	}
}
function ResetPasswordCallback(model)
{
	if (model)
	{
		alert("비밀번호가 변경되었습니다.");
		window.location.href = "/User/Login";
	}
	else alert("비밀번호 변경에 실패했습니다.\n존재하지 않는 사용자입니다.");
}

function Initialize()
{
	var header = parseFloat($("header").height()) + parseFloat($("header").css("paddingTop")) * 2;
	var collapse = $(".collapse").css("display") == "none" || $(window).width() >= 1200 ? 0 : parseFloat($(".collapse").height() + parseFloat($(".collapse").css("paddingTop")) * 2);
	var padding = parseFloat($(".full-screen-area").css("paddingTop"));

	$("article").height(window.innerHeight - header + collapse - (padding * 2));
	$(window).resize(function ()
	{
		var header = parseFloat($("header").height()) + parseFloat($("header").css("paddingTop")) * 2;
		var collapse = $(".collapse").css("display") == "none" || $(window).width() >= 1200 ? 0 : parseFloat($(".collapse").height() + parseFloat($(".collapse").css("paddingTop")) * 2);
		var padding = parseFloat($(".full-screen-area").css("paddingTop"));

		$("article").height(window.innerHeight - header + collapse - (padding * 2));
	});
}

$(document).ready(function ()
{
	Initialize();

	$("#phone").on("keyup", function ()
	{
		var value = $("#phone").val();
		if (value.length > 0)
		{
			if (ValidatePhone(value))
			{
				$("#phone").parent().siblings("label").html("인증이 필요합니다.");
				$("#phone").parent().siblings("label").removeClass("text-success");
				$("#phone").parent().siblings("label").addClass("text-danger");
				$("#phone").removeClass("is-valid");
				$("#phone").addClass("is-invalid");
				$("#validate-phone").prop("disabled", false);
			}
			else
			{
				$("#phone").parent().siblings("label").html("유효하지 않은 번호입니다.");
				$("#phone").parent().siblings("label").removeClass("text-success");
				$("#phone").parent().siblings("label").addClass("text-danger");
				$("#phone").removeClass("is-valid");
				$("#phone").addClass("is-invalid");
				$("#validate-phone").prop("disabled", true);
			}
		}
		else
		{
			$("#phone").removeClass("is-invalid");
			$("#phone").removeClass("is-valid");
			$("#phone").parent().siblings("label").html("휴대폰 번호");
			$("#phone").parent().siblings("label").removeClass("text-danger");
			$("#phone").parent().siblings("label").removeClass("text-success");
			$("#validate-phone").prop("disabled", true);
		}
	});
	$("#validate-phone").on("click", function (e)
	{
		e.preventDefault();
		$("#phone").prop("disabled", true);
		Ajax("/User/VerifyPhoneForUser", {
			phone: $("#phone").val()
		}, VerifyPhoneCallback);
	});
	$("#code").on("keyup", function ()
	{
		$("#code").val($("#code").val().replace(/[^0-9]/gi, ""));
		var value = $("#code").val();
		if (value.length == 6) $("#challenge").prop("disabled", false);
		else $("#challenge").prop("disabled", true);
	});
	$("#challenge").on("click", function ()
	{
		$("#code").prop("disabled", true);
		Ajax("/User/Challenge", {
			phone: $("#phone").val(),
			code: $("#code").val()
		}, ChallengeCallback);
	});
	$("#new-password").on("keyup", function ()
	{
		var origin = $("#new-password").val();
		if (origin.length > 0)
		{
			if (ValidatePassword(origin))
			{
				$("#new-password").siblings("label").html("안전한 비밀번호입니다.");
				$("#new-password").siblings("label").removeClass("text-danger");
				$("#new-password").siblings("label").addClass("text-success");
				$("#new-password").removeClass("is-invalid");
				$("#new-password").addClass("is-valid");
			}
			else
			{
				$("#new-password").siblings("label").html("영문자, 숫자 포함 4 ~ 20자리 입니다.");
				$("#new-password").siblings("label").removeClass("text-success");
				$("#new-password").siblings("label").addClass("text-danger");
				$("#new-password").removeClass("is-valid");
				$("#new-password").addClass("is-invalid");
			}
		}
		else
		{
			$("#new-password").siblings("label").html("새 비밀번호");
			$("#new-password").siblings("label").removeClass("text-danger");
			$("#new-password").siblings("label").removeClass("text-success");
			$("#new-password").removeClass("is-invalid");
			$("#new-password").removeClass("is-valid");
		}
		ValidateForm();
		$("#confirm-password").trigger("keyup");
	});
	$("#confirm-password").on("keyup", function ()
	{
		var origin = $("#new-password").val();
		var confirm = $("#confirm-password").val();
		if (origin.length > 0)
		{
			if ($("#new-password").hasClass("is-valid"))
			{
				if (origin == confirm)
				{
					$("#confirm-password").siblings("label").html("비밀번호가 일치합니다.");
					$("#confirm-password").siblings("label").removeClass("text-danger");
					$("#confirm-password").siblings("label").addClass("text-success");
					$("#confirm-password").removeClass("is-invalid");
					$("#confirm-password").addClass("is-valid");
				}
				else
				{
					$("#confirm-password").siblings("label").html("비밀번호가 일치하지 않습니다.");
					$("#confirm-password").siblings("label").removeClass("text-success");
					$("#confirm-password").siblings("label").addClass("text-danger");
					$("#confirm-password").removeClass("is-valid");
					$("#confirm-password").addClass("is-invalid");
				}
			}
			else
			{
				$("#confirm-password").siblings("label").html("잘못된 비밀번호입니다.");
				$("#confirm-password").siblings("label").removeClass("text-success");
				$("#confirm-password").siblings("label").addClass("text-danger");
				$("#confirm-password").removeClass("is-valid");
				$("#confirm-password").addClass("is-invalid");
			}
		}
		else
		{
			$("#confirm-password").siblings("label").html("비밀번호 확인");
			$("#confirm-password").siblings("label").removeClass("text-danger");
			$("#confirm-password").siblings("label").removeClass("text-success");
			$("#confirm-password").removeClass("is-invalid");
			$("#confirm-password").removeClass("is-valid");
		}
		ValidateForm();
	});
	$("#change-password").on("click", function ()
	{
		Ajax("/User/ResetPassword", {
			token: token,
			newPassword: $("#new-password").val()
		}, ResetPasswordCallback);
	});

	$("#phone").focus();
});