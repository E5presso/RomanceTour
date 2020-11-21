var popUpOpened = false;

function GetFormCallback(model)
{
	$("#form-area").html(model);
	$("img").attr("draggable", false);
	$("img").on("dragstart", function (e) { e.preventDefault(); });
}
function Initialize()
{
	var formUrl = $("#form-url").val();
	var fileName = formUrl.split("=")[1];
	AjaxWithoutLoading("/Product/GetForm", {
		fileName: fileName
	}, GetFormCallback);
}

$(document).ready(function ()
{
	$(".guide-area").on("touchmove mousewheel DOMMouseScroll scroll", function (e)
	{
		var e = e.originalEvent;
		var delta = (e.detail < 0 || e.wheelDelta > 0) ? 1 : -1;
		if (delta < 0) $(this).hide();
		e.preventDefault();
	});
	$(".edit-product").click(function ()
	{
		var id = $("#product-id").val();
		window.location.href = `/Admin/EditProduct?id=${id}`;
	});
	$("#make-appointment").on("click", function ()
	{
		var id = $("#product-id").val();
		window.location.href = `/Appointment/AddUserAppointment?id=${id}`;
	});

	Initialize();
});