var isPopupOpened = false;

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
function ValidatePassword(passwd) {
	var regex = /^(?=.*[a-zA-Z])(?=.*[0-9]).{4,20}$/;
	return regex.test(passwd);
}
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
		if ($(".id").hasClass("is-valid") &&
			$(".password").hasClass("is-valid") &&
			$(".confirm-password").hasClass("is-valid") &&
			$(".name").hasClass("is-valid") &&
			$(".address").hasClass("is-valid") &&
			$(".phone").hasClass("is-valid") &&
			$(".birthday").hasClass("is-valid"))
			$(".create-account").prop("disabled", false);
		else $(".create-account").prop("disabled", true);
	}, 10);
}

function UpdateAccountCallback(model) {
	if (model)
	{
		alert("회원가입이 완료되었습니다.");
		window.location.href = "/Home/Index";
	}
	else alert("이미 존재하는 사용자입니다.");
}

$(document).ready(function () {
	Initialize();

	$(".id").on("keyup", function () {
		var id = $(".id").val();
		if (id.length > 0)
		{
			AjaxWithoutLoading("/User/CheckDuplication", {
				UserName: id
			}, function (model) {
				if (model)
				{
					$(".id").removeClass("is-invalid");
					$(".id").addClass("is-valid");
					$(".id").siblings("label").html("사용 가능한 아이디입니다.");
					$(".id").siblings("label").removeClass("text-danger");
					$(".id").siblings("label").addClass("text-success");
				}
				else
				{
					$(".id").removeClass("is-valid");
					$(".id").addClass("is-invalid");
					$(".id").siblings("label").html("이미 가입된 사용자입니다.");
					$(".id").siblings("label").removeClass("text-success");
					$(".id").siblings("label").addClass("text-danger");
				}
			});
		}
		else
		{
			$(".id").removeClass("is-invalid");
			$(".id").removeClass("is-valid");
			$(".id").siblings("label").html("아이디");
			$(".id").siblings("label").removeClass("text-danger");
			$(".id").siblings("label").removeClass("text-success");
		}
		ValidateForm();
	});
	$(".password").on("keyup", function () {
		var origin = $(".password").val();
		if (origin.length > 0)
		{
			if (ValidatePassword(origin))
			{
				$(".password").siblings("label").html("안전한 비밀번호입니다.");
				$(".password").siblings("label").removeClass("text-danger");
				$(".password").siblings("label").addClass("text-success");
				$(".password").removeClass("is-invalid");
				$(".password").addClass("is-valid");
			}
			else
			{
				$(".password").siblings("label").html("영문자, 숫자 포함 4 ~ 20자리 입니다.");
				$(".password").siblings("label").removeClass("text-success");
				$(".password").siblings("label").addClass("text-danger");
				$(".password").removeClass("is-valid");
				$(".password").addClass("is-invalid");
			}
		}
		else
		{
			$(".password").siblings("label").html("비밀번호");
			$(".password").siblings("label").removeClass("text-danger");
			$(".password").siblings("label").removeClass("text-success");
			$(".password").removeClass("is-invalid");
			$(".password").removeClass("is-valid");
		}
		ValidateForm();
		$(".confirm-password").trigger("keyup");
	});
	$(".confirm-password").on("keyup", function () {
		var origin = $(".password").val();
		var confirm = $(".confirm-password").val();
		if (origin.length > 0)
		{
			if ($(".password").hasClass("is-valid"))
			{
				if (origin == confirm)
				{
					$(".confirm-password").siblings("label").html("비밀번호가 일치합니다.");
					$(".confirm-password").siblings("label").removeClass("text-danger");
					$(".confirm-password").siblings("label").addClass("text-success");
					$(".confirm-password").removeClass("is-invalid");
					$(".confirm-password").addClass("is-valid");
				}
				else
				{
					$(".confirm-password").siblings("label").html("비밀번호가 일치하지 않습니다.");
					$(".confirm-password").siblings("label").removeClass("text-success");
					$(".confirm-password").siblings("label").addClass("text-danger");
					$(".confirm-password").removeClass("is-valid");
					$(".confirm-password").addClass("is-invalid");
				}
			}
			else
			{
				$(".confirm-password").siblings("label").html("잘못된 비밀번호입니다.");
				$(".confirm-password").siblings("label").removeClass("text-success");
				$(".confirm-password").siblings("label").addClass("text-danger");
				$(".confirm-password").removeClass("is-valid");
				$(".confirm-password").addClass("is-invalid");
			}
		}
		else
		{
			$(".confirm-password").siblings("label").html("비밀번호 확인");
			$(".confirm-password").siblings("label").removeClass("text-danger");
			$(".confirm-password").siblings("label").removeClass("text-success");
			$(".confirm-password").removeClass("is-invalid");
			$(".confirm-password").removeClass("is-valid");
		}
		ValidateForm();
	});
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
	$(".address").on("focus", function () {
		if (!isPopupOpened)
		{
			new daum.Postcode({
				oncomplete: function (result) {
					$(".address").val(result.address);
					var address = $(".address").val();
					if (address.length)
					{
						$(".address").siblings("label").html("유효한 주소입니다.");
						$(".address").siblings("label").removeClass("text-danger");
						$(".address").siblings("label").addClass("text-success");
						$(".address").removeClass("is-invalid");
						$(".address").addClass("is-valid");
					}
					else
					{
						$(".address").siblings("label").html("주소를 입력해주세요.");
						$(".address").siblings("label").removeClass("text-success");
						$(".address").siblings("label").addClass("text-danger");
						$(".address").removeClass("is-valid");
						$(".address").addClass("is-invalid");
					}
					ValidateForm();
				},
				onclose: function () {
					isPopupOpened = false;
					$(".phone").focus();
				}
			}).open();
			isPopupOpened = true;
		}
	});
	$(".phone").on("keyup", function () {
		var value = $(".phone").val();
		if (value.length > 0)
		{
			if (ValidatePhone(value))
			{
				$(".phone").parent().siblings("label").html("유효한 번호입니다.");
				$(".phone").parent().siblings("label").removeClass("text-danger");
				$(".phone").parent().siblings("label").addClass("text-success");
				$(".phone").removeClass("is-invalid");
				$(".phone").addClass("is-valid");
				$(".validate-phone").prop("disabled", false);
			}
			else
			{
				$(".phone").parent().siblings("label").html("유효하지 않은 번호입니다.");
				$(".phone").parent().siblings("label").removeClass("text-success");
				$(".phone").parent().siblings("label").addClass("text-danger");
				$(".phone").removeClass("is-valid");
				$(".phone").addClass("is-invalid");
				$(".validate-phone").prop("disabled", true);
			}
		}
		else
		{
			$(".phone").removeClass("is-invalid");
			$(".phone").removeClass("is-valid");
			$(".phone").parent().siblings("label").html("휴대폰 번호");
			$(".phone").parent().siblings("label").removeClass("text-danger");
			$(".phone").parent().siblings("label").removeClass("text-success");
			$(".validate-phone").prop("disabled", true);
		}
		ValidateForm();
	});
	$(".birthday").on("change", function () {
		var value = $(".birthday").val();
		if (value.length > 0)
		{
			$(".birthday").siblings("label").html("유효한 날짜입니다.");
			$(".birthday").siblings("label").removeClass("text-danger");
			$(".birthday").siblings("label").addClass("text-success");
			$(".birthday").removeClass("is-invalid");
			$(".birthday").addClass("is-valid");
		}
		else
		{
			$(".birthday").siblings("label").html("유효하지 않은 날짜입니다.");
			$(".birthday").siblings("label").removeClass("text-success");
			$(".birthday").siblings("label").addClass("text-danger");
			$(".birthday").removeClass("is-valid");
			$(".birthday").addClass("is-invalid");
		}
		ValidateForm();
	});
	$(".create-account").on("click", function (e) {
		e.preventDefault();

		var id = $(".id").val();
		var password = $(".password").val();
		var name = $(".name").val();
		var address = $(".address").val();
		var phone = $(".phone").val();
		var birthday = $(".birthday").val();
		Ajax("/User/CreateAccount", {
			UserName: id,
			Password: password,
			Name: name,
			Address: address,
			Phone: phone,
			Birthday: birthday
		}, UpdateAccountCallback);
	});

	$(".id").focus();
});