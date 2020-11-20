var allowTermsAndConditions = false;
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
function ValidatePhone(phone)
{
	var regex = /(01[016789])([1-9]{1}[0-9]{2,3})([0-9]{4})$/;
	return regex.test(phone);
}

function ValidatePrimaryInfo()
{
	setTimeout(function ()
	{
		if ($("#username").hasClass("is-valid") &&
			$("#password").hasClass("is-valid") &&
			$("#confirm-password").hasClass("is-valid"))
			$("#primary-info .next-step").prop("disabled", false);
		else $("#primary-info .next-step").prop("disabled", true);
	}, 10);
}
function ValidatePersonalInfo()
{
	setTimeout(function ()
	{
		if ($("#name").hasClass("is-valid") &&
			$("#birthday").hasClass("is-valid") &&
			$("#address").hasClass("is-valid") &&
			$("#phone").hasClass("is-valid"))
			$("#personal-info .next-step").prop("disabled", false);
		else $("#personal-info .next-step").prop("disabled", true);
	}, 10);
}
function ValidatePaymentInfo()
{
	setTimeout(function ()
	{
		if ($("#billing-name").hasClass("is-valid") &&
			$("#billing-bank").hasClass("is-valid") &&
			$("#billing-number").hasClass("is-valid"))
			$("#payment-info .next-step").prop("disabled", false);
		else $("#payment-info .next-step").prop("disabled", true);
	}, 10);
}
function ValidateTermsAndConditions()
{
	setTimeout(function ()
	{
		if (allowTermsAndConditions)
		{
			$("#allow-marketing-promotions").prop("disabled", false);
			$("#create-account").prop("disabled", false);
		}
		else
		{
			allowMarketingPromotions = false;
			$("#allow-marketing-promotions").prop("disabled", true);
			$("#allow-marketing-promotions").prop("checked", false);
			$("#create-account").prop("disabled", true);
		}
	}, 10);
}

function CreateAccountCallback(model)
{
	if (model)
	{
		alert("회원가입이 완료되었습니다.");
		window.location.href = "/Home/Index";
	}
	else alert("이미 존재하는 사용자입니다.");
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
		ValidatePersonalInfo();
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

	// 기본정보
	$("#username").on("keyup", function ()
	{
		var id = $("#username").val();
		if (id.length > 0)
		{
			AjaxWithoutLoading("/User/CheckDuplication", {
				UserName: id
			}, function (model)
			{
				if (model)
				{
					$("#username").removeClass("is-invalid");
					$("#username").addClass("is-valid");
					$("#username").siblings("label").html("사용 가능한 아이디입니다.");
					$("#username").siblings("label").removeClass("text-danger");
					$("#username").siblings("label").addClass("text-success");
				}
				else
				{
					$("#username").removeClass("is-valid");
					$("#username").addClass("is-invalid");
					$("#username").siblings("label").html("이미 가입된 사용자입니다.");
					$("#username").siblings("label").removeClass("text-success");
					$("#username").siblings("label").addClass("text-danger");
				}
			});
		}
		else
		{
			$("#username").removeClass("is-invalid");
			$("#username").removeClass("is-valid");
			$("#username").siblings("label").html("아이디");
			$("#username").siblings("label").removeClass("text-danger");
			$("#username").siblings("label").removeClass("text-success");
		}
		ValidatePrimaryInfo();
	});
	$("#password").on("keyup", function ()
	{
		var origin = $("#password").val();
		if (origin.length > 0)
		{
			if (ValidatePassword(origin))
			{
				$("#password").siblings("label").html("안전한 비밀번호입니다.");
				$("#password").siblings("label").removeClass("text-danger");
				$("#password").siblings("label").addClass("text-success");
				$("#password").removeClass("is-invalid");
				$("#password").addClass("is-valid");
			}
			else
			{
				$("#password").siblings("label").html("영문자, 숫자 포함 4 ~ 20자리 입니다.");
				$("#password").siblings("label").removeClass("text-success");
				$("#password").siblings("label").addClass("text-danger");
				$("#password").removeClass("is-valid");
				$("#password").addClass("is-invalid");
			}
		}
		else
		{
			$("#password").siblings("label").html("비밀번호");
			$("#password").siblings("label").removeClass("text-danger");
			$("#password").siblings("label").removeClass("text-success");
			$("#password").removeClass("is-invalid");
			$("#password").removeClass("is-valid");
		}
		ValidatePrimaryInfo();
		$("#confirm-password").trigger("keyup");
	});
	$("#confirm-password").on("keyup", function ()
	{
		var origin = $("#password").val();
		var confirm = $("#confirm-password").val();
		if (origin.length > 0)
		{
			if ($("#password").hasClass("is-valid"))
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
		ValidatePrimaryInfo();
	});
	$("#primary-info .next-step").on("click", function ()
	{
		$("#primary-info").fadeOut(DURATION).removeClass("enabled");
		$("#personal-info-step").addClass("activated");
		$("#personal-info").fadeIn(DURATION).addClass("enabled");
		$("#name").focus();
	});

	// 개인정보
	$("#name").on("keyup", function ()
	{
		var value = $("#name").val();
		if (value.length > 0)
		{
			if (ValidateName(value))
			{
				$("#name").siblings("label").html("유효한 이름입니다.");
				$("#name").siblings("label").removeClass("text-danger");
				$("#name").siblings("label").addClass("text-success");
				$("#name").removeClass("is-invalid");
				$("#name").addClass("is-valid");
			}
			else
			{
				$("#name").siblings("label").html("유효하지 않은 이름입니다.");
				$("#name").siblings("label").removeClass("text-success");
				$("#name").siblings("label").addClass("text-danger");
				$("#name").removeClass("is-valid");
				$("#name").addClass("is-invalid");
			}
		}
		else
		{
			$("#name").removeClass("is-invalid");
			$("#name").removeClass("is-valid");
			$("#name").siblings("label").html("이름");
			$("#name").siblings("label").removeClass("text-danger");
			$("#name").siblings("label").removeClass("text-success");
		}
		ValidatePersonalInfo();
	});
	$("#birthday").on("change", function ()
	{
		var value = $("#birthday").val();
		if (value.length > 0)
		{
			$("#birthday").siblings("label").html("유효한 날짜입니다.");
			$("#birthday").siblings("label").removeClass("text-danger");
			$("#birthday").siblings("label").addClass("text-success");
			$("#birthday").removeClass("is-invalid");
			$("#birthday").addClass("is-valid");
		}
		else
		{
			$("#birthday").siblings("label").html("유효하지 않은 날짜입니다.");
			$("#birthday").siblings("label").removeClass("text-success");
			$("#birthday").siblings("label").addClass("text-danger");
			$("#birthday").removeClass("is-valid");
			$("#birthday").addClass("is-invalid");
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
					ValidatePersonalInfo();
				},
				onclose: function ()
				{
					isPopupOpened = false;
					$("#phone").focus();
				}
			}).open();
			isPopupOpened = true;
		}
	});
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
		ValidatePersonalInfo();
	});
	$("#validate-phone").on("click", function ()
	{
		$("#phone").prop("disabled", true);
		Ajax("/User/VerifyPhone", {
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
	$("#personal-info .next-step").on("click", function ()
	{
		$("#personal-info").fadeOut(DURATION).removeClass("enabled");
		$("#payment-info-step").addClass("activated");
		$("#payment-info").fadeIn(DURATION).addClass("enabled");
		$("#billing-name").focus();
	});

	// 결제정보
	$("#billing-name").on("keyup", function ()
	{
		var value = $("#billing-name").val();
		if (value.length > 0)
		{
			if (ValidateName(value))
			{
				$("#billing-name").siblings("label").html("유효한 이름입니다.");
				$("#billing-name").siblings("label").removeClass("text-danger");
				$("#billing-name").siblings("label").addClass("text-success");
				$("#billing-name").removeClass("is-invalid");
				$("#billing-name").addClass("is-valid");
			}
			else
			{
				$("#billing-name").siblings("label").html("유효하지 않은 이름입니다.");
				$("#billing-name").siblings("label").removeClass("text-success");
				$("#billing-name").siblings("label").addClass("text-danger");
				$("#billing-name").removeClass("is-valid");
				$("#billing-name").addClass("is-invalid");
			}
		}
		else
		{
			$("#billing-name").removeClass("is-invalid");
			$("#billing-name").removeClass("is-valid");
			$("#billing-name").siblings("label").html("예금주명");
			$("#billing-name").siblings("label").removeClass("text-danger");
			$("#billing-name").siblings("label").removeClass("text-success");
		}
		ValidatePaymentInfo();
	});
	$("#billing-bank").on("change", function ()
	{
		var value = $("#billing-bank").val();
		if (value.length > 0)
		{
			$("#billing-bank").siblings("label").html("은행이 선택되었습니다.");
			$("#billing-bank").siblings("label").removeClass("text-danger");
			$("#billing-bank").siblings("label").addClass("text-success");
			$("#billing-bank").removeClass("is-invalid");
			$("#billing-bank").addClass("is-valid");
		}
		else
		{
			$("#billing-bank").siblings("label").html("은행을 선택해주세요.");
			$("#billing-bank").siblings("label").removeClass("text-success");
			$("#billing-bank").siblings("label").addClass("text-danger");
			$("#billing-bank").removeClass("is-valid");
			$("#billing-bank").addClass("is-invalid");
		}
		ValidatePaymentInfo();
	});
	$("#billing-number").on("keyup", function ()
	{
		var value = $("#billing-number").val();
		if (value.length > 0)
		{
			$("#billing-number").siblings("label").html("계좌번호가 입력되었습니다.");
			$("#billing-number").siblings("label").removeClass("text-danger");
			$("#billing-number").siblings("label").addClass("text-success");
			$("#billing-number").removeClass("is-invalid");
			$("#billing-number").addClass("is-valid");
		}
		else
		{
			$("#billing-number").siblings("label").html("계좌번호를 입력해주세요.");
			$("#billing-number").siblings("label").removeClass("text-success");
			$("#billing-number").siblings("label").addClass("text-danger");
			$("#billing-number").removeClass("is-valid");
			$("#billing-number").addClass("is-invalid");
		}
		ValidatePaymentInfo();
	});
	$("#payment-info .next-step").on("click", function ()
	{
		$("#payment-info").fadeOut(DURATION).removeClass("enabled");
		$("#terms-and-conditions-step").addClass("activated");
		$("#terms-and-conditions").fadeIn(DURATION).addClass("enabled");
	});

	// 이용약관
	$("#allow-terms-and-conditions").on("click", function ()
	{
		allowTermsAndConditions = !allowTermsAndConditions;
		ValidateTermsAndConditions();
	});
	$("#allow-marketing-promotions").on("click", function ()
	{
		allowMarketingPromotions = !allowMarketingPromotions;
		ValidateTermsAndConditions();
	});
	$("#create-account").on("click", function (e)
	{
		e.preventDefault();
		if (token == "")
		{
			alert("휴대폰 인증을 완료해주세요.");
			$("#phone").focus();
		}
		else
		{
			var username = $("#username").val();
			var password = $("#password").val();

			var name = $("#name").val();
			var birthday = $("#birthday").val();
			var address = $("#address").val();
			var phone = $("#phone").val();

			var billingName = $("#billing-name").val();
			var billingBank = $("#billing-bank").val();
			var billingNumber = $("#billing-number").val();

			Ajax("/User/CreateAccount", {
				token: token,
				user: {
					UserName: username,
					Password: password,
					Name: name,
					Birthday: birthday,
					Address: address,
					Phone: phone,
					BillingName: billingName,
					BillingBank: billingBank,
					BillingNumber: billingNumber,
					AllowTermsAndConditions: allowTermsAndConditions,
					AllowMarketingPromotions: allowMarketingPromotions
				}
			}, CreateAccountCallback);
		}
	});

	$("#username").focus();
});