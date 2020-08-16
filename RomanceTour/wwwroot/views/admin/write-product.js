var datepicker;

function UploadCallback(model) {
	if (model)
	{
		alert("상품이 추가되었습니다.");
		window.location.href = "/Admin/ManageProduct";
	}
	else alert("상품 등록에 문제가 발생하였습니다.\n입력이 비어있는지는 확인해보셨나요? '^'");
}

$(document).ready(function () {
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

	$(".go-back").on("click", function () {
		window.location.href = "/Admin/ManageProduct";
	});
	$(".custom-file-input").on("change", function () {
		var fileName = $(this).val().split("\\").pop();
		$(this).siblings(".custom-file-label").addClass("selected").html(fileName);
	});
	$(".submit-post").on("click", function () {
		var title = $("#title").val();
		var subTitle = $("#sub-title").val();
		var category = $("#category option:selected").val();
		var thumbnail = $("#thumbnail-image")[0].files[0];
		var price = $("#price").val();
		var form = CKEDITOR.instances.editor.getData();

		if (form.length > 0)
		{
			var data = new FormData();
			data.append("Title", title);
			data.append("SubTitle", subTitle);
			data.append("CategoryId", category);
			data.append("Thumbnail", thumbnail);
			data.append("Price", price);
			$("#price-rule").find("option:selected").each(function (index, item) {
				data.append(`PriceRules[${index}]`, $(item).val());
			});
			$("#departure").find("option:selected").each(function (index, item) {
				data.append(`Departures[${index}]`, $(item).val());
			});
			$("#host").find("option:selected").each(function (index, item) {
				data.append(`Hosts[${index}]`, $(item).val());
			});
			$("#billing").find("option:selected").each(function (index, item) {
				data.append(`Billings[${index}]`, $(item).val());
			});
			$.each(datepicker.selectedDates, function (index, item) {
				data.append(`Appointments[${index}]`, item.toISOString());
			});
			data.append("Form", form);

			AjaxForm("/Product/AddProduct", data, UploadCallback);
		}
		else alert("게시글 내용이 비어있어요 '^'");
	});
});