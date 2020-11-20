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
function ValidateForm()
{
	var id = $(".id").val();
	var password = $(".password").val();

	if (id.length > 0 && password.length > 0)
		$(".check-account").prop("disabled", false);
	else $(".check-account").prop("disabled", true);
}
function CheckAccountCallback(model)
{
	if (model.Result)
	{
		alert(model.Message);
		var back = $("#back").val();
		window.location.href = back;
	}
	else
	{
		alert(model.Message);
		$(".password").val('');
		$(".password").focus();
		$(".check-account").prop("disabled", true);
	}
}

$(document).ready(function ()
{
	Initialize();
	$(".id").on("keyup", function ()
	{
		ValidateForm();
	});
	$(".password").on("keyup", function ()
	{
		ValidateForm();
	});
	$(".check-account").on("click", function (e)
	{
		e.preventDefault();

		var id = $(".id").val();
		var pw = $(".password").val();
		Ajax("/User/CheckAccount", {
			UserName: id,
			Password: pw
		}, CheckAccountCallback);
	});
	$(".register").on("click", function (e)
	{
		e.preventDefault();
		window.location.href = "/User/Register";
	});
	$("#forgot-password").on("click", function ()
	{
		window.location.href = "/User/FindMyPassword";
	});

	ValidateForm();
	$(".id").focus();
});