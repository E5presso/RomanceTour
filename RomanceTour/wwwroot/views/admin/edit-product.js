var datepicker;

function UploadCallback(model)
{
	if (model)
	{
		alert("상품이 편집되었습니다.");
		var back = $("#back").val();
		window.location.href = back;
	}
	else alert("상품 편집에 문제가 발생하였습니다.\n입력이 비어있는지는 확인해보셨나요? '^'");
}
function GetProductCallback(model)
{
	console.log(model.Form);
	var form = model.Form;
	var fileName = form.split("=")[1];
	console.log(fileName);
	AjaxWithoutLoading("/Product/GetForm", {
		fileName: fileName
	}, GetFormCallback);
}
function GetFormCallback(model)
{
	CKEDITOR.instances.editor.setData(model);
	setTimeout(function ()
	{
		HideLoading();
	}, DELAY);
}
function GetAppointmentCallback(model)
{
	$.each(model, function (index, item)
	{
		datepicker.selectDate(new Date(item.Date));
	});
}
function Initialize()
{
	var id = $("#product-id").val();
	AjaxWithoutLoading("/Product/GetAppointment", {
		id: id
	}, GetAppointmentCallback);
	AjaxWithoutLoading("/Product/AjaxGetProduct", {
		id: id
	}, GetProductCallback);
}

$(document).ready(function ()
{
	ShowLoading();
	CKEDITOR.replace("editor");
	$(".multiple-select").selectpicker({
		noneSelectedText: "선택된 항목이 없습니다.",
		noneResultsText: "일치하는 항목이 없습니다."
	});
	datepicker = $("#available-appointment").datepicker({
		language: "ko",
		todayButton: true,
		allButton: true,
		clearButton: true,
		multipleDates: true,
	}).data('datepicker');

	$(".go-back").on("click", function ()
	{
		var back = $("#back").val();
		window.location.href = back;
	});
	$(".custom-file-input").on("change", function ()
	{
		var fileName = $(this).val().split("\\").pop();
		$(this).siblings(".custom-file-label").addClass("selected").html(fileName);
	});
	$(".submit-post").on("click", function ()
	{
		var id = $("#product-id").val();
		var title = $("#title").val();
		var subTitle = $("#sub-title").val();
		var category = $("#category option:selected").val();
		var thumbnail = $("#thumbnail-image")[0].files[0];
		var price = $("#price").val();
		var form = CKEDITOR.instances.editor.getData();

		if (form.length > 0)
		{
			var data = new FormData();
			data.append("Id", id);
			data.append("Title", title);
			data.append("SubTitle", subTitle);
			data.append("CategoryId", category);
			data.append("Thumbnail", thumbnail);
			data.append("Price", price);
			$("#price-rule").find("option:selected").each(function (index, item)
			{
				data.append(`PriceRules[${index}]`, $(item).val());
			});
			$("#departure").find("option:selected").each(function (index, item)
			{
				data.append(`Departures[${index}]`, $(item).val());
			});
			$("#host").find("option:selected").each(function (index, item)
			{
				data.append(`Hosts[${index}]`, $(item).val());
			});
			$("#billing").find("option:selected").each(function (index, item)
			{
				data.append(`Billings[${index}]`, $(item).val());
			});
			$.each(datepicker.selectedDates, function (index, item)
			{
				data.append(`Appointments[${index}]`, item.toISOString());
			});
			data.append("Form", form);

			AjaxForm("/Product/UpdateProduct", data, UploadCallback);
		}
		else alert("게시글 내용이 비어있어요 '^'");
	});

	Initialize();
});