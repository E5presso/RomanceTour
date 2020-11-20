var allowMarketingPromotions = false;

var isPopupOpened = false;
var VerificationResult = {
	SUCCESS: "SUCCESS",							// 발송 성공 시
	FAILURE: "FAILURE",							// 발송 실패 시
	TOO_MUCH_REQUEST: "TOO_MUCH_REQUEST",		// 너무 빠른 요청
	MAX_REQUEST_REACHED: "MAX_REQUEST_REACHED"	// 일일 요청한도 도달
};
var token = "";
var timeLimit = 3 * 60;
var timer;

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
function ValidatePassword(passwd)
{
	var regex = /^(?=.*[a-zA-Z])(?=.*[0-9]).{4,20}$/;
	return regex.test(passwd);
}
function ValidateName(name)
{
	var regex = /^[가-힣]{2,4}$/;
	return regex.test(name);
}
function ValidateAddress()
{
	var address = $("#address").val();
	if (address.length)
	{
		$("#address").siblings("label").html("유효한 주소입니다.");
		$("#address").siblings("label").removeClass("text-danger");
		$("#address").siblings("label").addClass("text-success");
		$("#address").removeClass("is-invalid");
		$("#address").addClass("is-valid");
	}
	else
	{
		$("#address").siblings("label").html("주소를 입력해주세요.");
		$("#address").siblings("label").removeClass("text-success");
		$("#address").siblings("label").addClass("text-danger");
		$("#address").removeClass("is-valid");
		$("#address").addClass("is-invalid");
	}
}
function ValidatePhone(phone)
{
	var regex = /(01[016789])([1-9]{1}[0-9]{2,3})([0-9]{4})$/;
	return regex.test(phone);
}

// 개인정보 변경
function ValidatePersonalInfo()
{
	var name = $("#name");
	var address = $("#address");
	var birthday = $("#birthday");

	if (name.hasClass("is-valid") && address.hasClass("is-valid") && birthday.hasClass("is-valid"))
		$("#change-personal-info").prop("disabled", false);
	else $("#change-personal-info").prop("disabled", true);

}
function ChangePersonalInfoCallback(model)
{
	if (model)
	{
		alert("개인정보 변경이 완료되었습니다.");
		location.reload(true);
	}
	else
	{
		alert("존재하지 않는 사용자입니다.");
		location.reload(true);
	}
}

// 휴대폰 번호 변경
function VerifyPhoneCallback(model)
{
	$("#new-phone-number").prop("disabled", false);
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
					$("#verification-form").children("label").text(`인증번호 [${Math.floor(timeLimit / 60).zf(2)}:${(timeLimit % 60).zf(2)}]`);
				}
				else
				{
					clearInterval(timer);
					$("#verification-form").children("label").removeClass("text-danger");
					$("#verification-code").removeClass("is-invalid");
					$("#verification-code").val("");
					$("#check-verification-code").prop("disabled", true);
					$("#verification-form").slideUp(DURATION);
					$("#new-phone-number").focus();
				}
			}, 1000);
			$("#verification-form").children("label").removeClass("text-danger");
			$("#verification-code").removeClass("is-invalid");
			$("#verification-code").val("");
			$("#check-verification-code").prop("disabled", true);
			alert("인증요청 문자를 발송했습니다.");
			$("#verification-form").slideDown(DURATION);
			$("#verification-code").focus();
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
	}
}
function CheckVerificationCodeCallback(model)
{
	$("#verification-code").prop("disabled", false);
	if (model.Result)
	{
		clearInterval(timer);
		token = model.Token;
		alert("휴대폰 인증에 성공하였습니다.");
		$("#new-phone-number").prop("disabled", true);
		$("#challenge").prop("disabled", true);
		$("#verification-form").slideUp(DURATION);
		$("#new-phone-number").parent().siblings("label").html("인증된 번호입니다.");
		$("#new-phone-number").parent().siblings("label").removeClass("text-danger");
		$("#new-phone-number").parent().siblings("label").addClass("text-success");
		$("#new-phone-number").removeClass("is-invalid");
		$("#new-phone-number").addClass("is-valid");
		$("#change-phone-number").prop("disabled", false);
	}
	else
	{
		alert("잘못된 인증번호입니다.");
		$("#verification-code").parent().siblings("label").html("잘못된 인증번호입니다.");
		$("#verification-code").parent().siblings("label").removeClass("text-success");
		$("#verification-code").parent().siblings("label").addClass("text-danger");
		$("#verification-code").addClass("is-invalid");
		$("#verification-code").removeClass("is-valid");
		$("#challenge").prop("disabled", false);
		$("#verification-code").focus();
	}
}
function ChangePhoneNumberCallback(model)
{
	if (model)
	{
		alert("번호 변경이 완료되었습니다.");
		location.reload(true);
	}
	else
	{
		alert("이미 사용 중인 번호입니다.");
		$("#new-phone-number").focus();
	}
}

// 결제정보 변경
function ValidatePaymentInfo()
{
	var billingName = $("#billing-name");
	var billingNumber = $("#billing-number");
	if (billingName.hasClass("is-valid") && billingNumber.val().length > 0)
		$("#change-payment-info").prop("disabled", false);
	else $("#change-payment-info").prop("disabled", true);
}
function ChangePaymentInfoCallback(model)
{
	if (model)
	{
		alert("결제정보 변경이 완료되었습니다.");
		location.reload(true);
	}
	else
	{
		alert("존재하지 않는 사용자입니다.");
		location.reload(true);
	}
}

// 비밀번호 변경
function ValidateChangePassword()
{
	var oldPassword = $("#old-password");
	var newPassword = $("#new-password");
	var confirmPassword = $("#confirm-password");

	if (oldPassword.val().length > 0 && newPassword.hasClass("is-valid") && confirmPassword.hasClass("is-valid"))
		$("#change-password").prop("disabled", false);
	else $("#change-password").prop("disabled", true);
}
function ChangePasswordCallback(model)
{
	if (model)
	{
		alert("비밀번호 변경이 완료되었습니다.");
		location.reload(true);
	}
	else
	{
		alert("잘못된 비밀번호입니다.\n이전 비밀번호를 다시 확인해주세요.");
		$("#old-password").val("");
		$("#old-password").focus();
	}
}

// 광고성 정보 수신 동의
function ChangeTermsAgreementCallback(model)
{
	if (model)
	{
		alert("마케팅 정보 수신에 대한 동의가 변경되었습니다.");
		location.reload(true);
	}
	else
	{
		alert("존재하지 않는 사용자입니다.");
		location.reload(true);
	}
}

// 회원 탈퇴
function ValidateUnregister()
{
	var username = $("#unregister-username").val();
	var password = $("#unregister-password").val();
	if (username.length > 0 && password.length > 0)
		$("#do-unregister").prop("disabled", false);
	else $("#do-unregister").prop("disabled", true);
}
function UnregisterCallback(model)
{
	if (model.Result)
	{
		alert(model.Message);
		location.href = "/Home/Index";
	}
	else
	{
		alert(model.Message);
		$("#unregister-username").val("");
		$("#unregister-password").val("");
		ValidateUnregister();
	}
}

$(document).ready(function ()
{
	Initialize();
	// 개인정보 변경
	$("#name").on("keyup", function ()
	{
		var value = $(this).val();
		if (value.length > 0)
		{
			if (ValidateName(value))
			{
				$(this).siblings("label").html("유효한 이름입니다.");
				$(this).siblings("label").removeClass("text-danger");
				$(this).siblings("label").addClass("text-success");
				$(this).removeClass("is-invalid");
				$(this).addClass("is-valid");
			}
			else
			{
				$(this).siblings("label").html("유효하지 않은 이름입니다.");
				$(this).siblings("label").removeClass("text-success");
				$(this).siblings("label").addClass("text-danger");
				$(this).removeClass("is-valid");
				$(this).addClass("is-invalid");
			}
		}
		else
		{
			$(this).removeClass("is-invalid");
			$(this).removeClass("is-valid");
			$(this).siblings("label").html("이름");
			$(this).siblings("label").removeClass("text-danger");
			$(this).siblings("label").removeClass("text-success");
		}
		ValidatePersonalInfo();
	});
	$("#address").on("focus", function ()
	{
		if (!isPopupOpened)
		{
			new daum.Postcode({
				oncomplete: function (result)
				{
					$("#address").val(result.address);
				},
				onclose: function ()
				{
					isPopupOpened = false;
					$("#birthday").focus();
					ValidateAddress();
					ValidatePersonalInfo();
				}
			}).open();
			isPopupOpened = true;
		}
	});
	$("#birthday").on("change", function ()
	{
		var value = $(this).val();
		if (value.length > 0)
		{
			$(this).siblings("label").html("유효한 날짜입니다.");
			$(this).siblings("label").removeClass("text-danger");
			$(this).siblings("label").addClass("text-success");
			$(this).removeClass("is-invalid");
			$(this).addClass("is-valid");
		}
		else
		{
			$(this).siblings("label").html("유효하지 않은 날짜입니다.");
			$(this).siblings("label").removeClass("text-success");
			$(this).siblings("label").addClass("text-danger");
			$(this).removeClass("is-valid");
			$(this).addClass("is-invalid");
		}
		ValidatePersonalInfo();
	});
	$("#change-personal-info").on("click", function ()
	{
		var name = $("#name").val();
		var address = $("#address").val();
		var birthday = $("#birthday").val();

		Ajax("/User/ChangePersonalInfo", {
			Name: name,
			Address: address,
			Birthday: birthday
		}, ChangePersonalInfoCallback);
	});

	// 휴대폰 번호 변경
	$("#new-phone-number").on("keyup", function ()
	{
		var value = $(this).val();
		if (value.length > 0)
		{
			if (ValidatePhone(value))
			{
				$(this).parent().siblings("label").html("인증이 필요합니다.");
				$(this).parent().siblings("label").removeClass("text-success");
				$(this).parent().siblings("label").addClass("text-danger");
				$(this).removeClass("is-valid");
				$(this).addClass("is-invalid");
				$("#challenge").prop("disabled", false);
			}
			else
			{
				$(this).parent().siblings("label").html("유효하지 않은 번호입니다.");
				$(this).parent().siblings("label").removeClass("text-success");
				$(this).parent().siblings("label").addClass("text-danger");
				$(this).removeClass("is-valid");
				$(this).addClass("is-invalid");
				$("#challenge").prop("disabled", true);
			}
		}
		else
		{
			$(this).removeClass("is-invalid");
			$(this).removeClass("is-valid");
			$(this).parent().siblings("label").html("새로운 휴대폰 번호");
			$(this).parent().siblings("label").removeClass("text-danger");
			$(this).parent().siblings("label").removeClass("text-success");
			$("#challenge").prop("disabled", true);
		}
	});
	$("#challenge").on("click", function ()
	{
		$("#new-phone-number").prop("disabled", true);
		Ajax("/User/VerifyPhone", {
			phone: $("#new-phone-number").val()
		}, VerifyPhoneCallback);
	});
	$("#verification-code").on("keyup", function ()
	{
		$(this).val($(this).val().replace(/[^0-9]/gi, ""));
		var value = $(this).val();
		if (value.length == 6) $("#check-verification-code").prop("disabled", false);
		else $("#check-verification-code").prop("disabled", true);
	});
	$("#check-verification-code").on("click", function ()
	{
		$("#verification-code").prop("disabled", true);
		Ajax("/User/Challenge", {
			phone: $("#new-phone-number").val(),
			code: $("#verification-code").val()
		}, CheckVerificationCodeCallback);
	});
	$("#change-phone-number").on("click", function ()
	{
		if (token == "")
		{
			alert("휴대폰 인증을 완료해주세요.");
			$("#new-phone-number").focus();
		}
		else
		{
			Ajax("/User/ChangePhoneNumber", {
				token: token
			}, ChangePhoneNumberCallback);
		}
	});

	// 결제정보 변경
	$("#billing-name").on("keyup", function ()
	{
		var value = $(this).val();
		if (value.length > 0)
		{
			if (ValidateName(value))
			{
				$(this).siblings("label").html("유효한 이름입니다.");
				$(this).siblings("label").removeClass("text-danger");
				$(this).siblings("label").addClass("text-success");
				$(this).removeClass("is-invalid");
				$(this).addClass("is-valid");
			}
			else
			{
				$(this).siblings("label").html("유효하지 않은 이름입니다.");
				$(this).siblings("label").removeClass("text-success");
				$(this).siblings("label").addClass("text-danger");
				$(this).removeClass("is-valid");
				$(this).addClass("is-invalid");
			}
		}
		else
		{
			$(this).removeClass("is-invalid");
			$(this).removeClass("is-valid");
			$(this).siblings("label").html("예금주명");
			$(this).siblings("label").removeClass("text-danger");
			$(this).siblings("label").removeClass("text-success");
		}
		ValidatePaymentInfo();
	});
	$("#billing-bank").on("change", function ()
	{
		ValidatePaymentInfo();
	});
	$("#billing-number").on("keyup", function ()
	{
		ValidatePaymentInfo();
	});
	$("#change-payment-info").on("click", function ()
	{
		var billingName = $("#billing-name").val();
		var billingBank = $("#billing-bank").children("option:selected").val();
		var billingNumber = $("#billing-number").val();

		Ajax("/User/ChangePaymentInfo", {
			BillingName: billingName,
			BillingBank: billingBank,
			BillingNumber: billingNumber
		}, ChangePaymentInfoCallback);
	});

	// 비밀번호 변경
	$("#old-password").on("keyup", function ()
	{
		ValidateChangePassword();
	});
	$("#new-password").on("keyup", function ()
	{
		var value = $(this).val();
		if (value.length > 0)
		{
			if (ValidatePassword(value))
			{
				$(this).siblings("label").html("안전한 비밀번호입니다.");
				$(this).siblings("label").removeClass("text-danger");
				$(this).siblings("label").addClass("text-success");
				$(this).removeClass("is-invalid");
				$(this).addClass("is-valid");
			}
			else
			{
				$(this).siblings("label").html("영문자, 숫자 포함 4 ~ 20자리 입니다.");
				$(this).siblings("label").removeClass("text-success");
				$(this).siblings("label").addClass("text-danger");
				$(this).removeClass("is-valid");
				$(this).addClass("is-invalid");
			}
		}
		else
		{
			$(this).removeClass("is-invalid");
			$(this).removeClass("is-valid");
			$(this).siblings("label").html("새 비밀번호");
			$(this).siblings("label").removeClass("text-danger");
			$(this).siblings("label").removeClass("text-success");
		}
		$("#confirm-password").trigger("keyup");
		ValidateChangePassword();
	});
	$("#confirm-password").on("keyup", function ()
	{
		var origin = $("#new-password").val();
		var confirm = $(this).val();
		if (origin.length > 0)
		{
			if ($("#new-password").hasClass("is-valid"))
			{
				if (origin == confirm)
				{
					$(this).siblings("label").html("비밀번호가 일치합니다.");
					$(this).siblings("label").removeClass("text-danger");
					$(this).siblings("label").addClass("text-success");
					$(this).removeClass("is-invalid");
					$(this).addClass("is-valid");
				}
				else
				{
					$(this).siblings("label").html("비밀번호가 일치하지 않습니다.");
					$(this).siblings("label").removeClass("text-success");
					$(this).siblings("label").addClass("text-danger");
					$(this).removeClass("is-valid");
					$(this).addClass("is-invalid");
				}
			}
			else
			{
				$(this).siblings("label").html("잘못된 비밀번호입니다.");
				$(this).siblings("label").removeClass("text-success");
				$(this).siblings("label").addClass("text-danger");
				$(this).removeClass("is-valid");
				$(this).addClass("is-invalid");
			}
		}
		else
		{
			$(this).siblings("label").html("비밀번호 확인");
			$(this).siblings("label").removeClass("text-danger");
			$(this).siblings("label").removeClass("text-success");
			$(this).removeClass("is-invalid");
			$(this).removeClass("is-valid");
		}
		ValidateChangePassword();
	});
	$("#change-password").on("click", function ()
	{
		var oldPassword = $("#old-password").val();
		var newPassword = $("#new-password").val();

		Ajax("/User/ChangePassword", {
			oldPassword: oldPassword,
			newPassword: newPassword
		}, ChangePasswordCallback);
	});

	// 광고성 정보 수신 동의
	$("#allow-marketing-promotions").on("click", function ()
	{
		allowMarketingPromotions = !allowMarketingPromotions;
		$("#change-agreement").prop("disabled", false);
	});
	$("#change-agreement").on("click", function ()
	{
		Ajax("/User/ChangeTermsAgreement", {
			agreement: allowMarketingPromotions
		}, ChangeTermsAgreementCallback);
	});

	// 회원 탈퇴
	$("#unregister-username").on("keyup", function ()
	{
		ValidateUnregister();
	});
	$("#unregister-password").on("keyup", function ()
	{
		ValidateUnregister();
	});
	$("#do-unregister").on("click", function ()
	{
		var username = $("#unregister-username").val();
		var password = $("#unregister-password").val();

		Ajax("/User/Unregister", {
			UserName: username,
			Password: password
		}, UnregisterCallback);
	});

	allowMarketingPromotions = $("#allow-marketing-promotions").is(":checked");
});