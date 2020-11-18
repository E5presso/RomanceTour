var min = 10000;
var max = 150000;
var step = 10000;

var appointment;
var datepicker;
var fromAppointment = null;
var toAppointment = null;

function AppointmentUpdateCallback(data)
{
	fromAppointment = data.from;
	toAppointment = data.to;
}
function AppointmentChangeCallback(data)
{
	fromAppointment = data.from;
	toAppointment = data.to;
}

function FilterProductCallback(model)
{
	$(".product-list").html('');
	if (model.length > 0) $.each(model, function (index, item)
	{
		var keyword = $("#product-keyword").val();
		var product = `
			<li id="product-${item.Id}" class="product-item">
				<img class="product-thumbnail" src="${item.Thumbnail}" draggable="false" />
				<div class="product-info container-fluid">
					<div class="row">
						<span class="product-title col col-7">
							<span class="inner-frame">
								${item.Title}
							</span>
						</span>
						<span class="product-date col col-5">
							${new Date(item.FastAvailable).format(`yyyy.MM.dd`)} ~
						</span>
						<div class="product-sub-area col col-12 container-fluid">
							<div class="row">
								<span class="product-subtitle col col-7">
									<span class="inner-frame">
										${item.SubTitle}
									</span>
								</span>
								<div class="product-sub-area col col-5">
									<div class="row">
										<span class="product-price col col-12">
											&#8361; ${item.Price.format()}
										</span>
										<span class="product-confirmed col col-12">
											${item.Confirmed ? `
												<span class="inner-frame">
													출발확정
												</span>
											` : ``}
										</span>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</li>
        `;
		if (keyword) product = product.replaceAll(keyword, `<span class="highlighted">${keyword}</span>`);
		$(".product-list").append(product);
		$(`#product-${item.Id}`).on("click", function ()
		{
			var id = $(this).attr("id").split("-")[1];
			window.location.href = `/Product/GetProduct?id=${id}`;
		});
		$("img").attr("draggable", false);
		$("img").on("dragstart", function (e) { e.preventDefault(); });
	});
	else $(".product-list").append(`<span>검색조건에 맞는 상품을 찾을 수 없습니다.</span>`);
}
function FilterProduct()
{
	var category = $("#category-select").children("option:selected").val();
	var sorting = $("#sorting-select").children("option:selected").val();
	var keyword = $("#product-keyword").val();

	AjaxWithoutLoading("/Product/FilterProduct", {
		category: category,
		filter: {
			Sorting: sorting,
			Keyword: keyword,
			FromPrice: fromAppointment,
			ToPrice: toAppointment,
			Date: datepicker.selectedDates[0] ? datepicker.selectedDates[0].toISOString() : null,
			Confirmed: $("#product-confirmed").is(":checked")
		}
	}, FilterProductCallback);
}
function Initialize()
{
	$(".range-slider").ionRangeSlider({
		type: "double",
		min: min,
		max: max,
		step: step,
		prettify_enabled: true,
		prettify_separator: ",",
		prefix: "&#8361;",
		values_separator: " ~ ",
		onChange: AppointmentChangeCallback,
		onUpdate: AppointmentUpdateCallback
	});

	var date = $("#date").val();
	appointment = $(".range-slider").data("ionRangeSlider");
	datepicker = $(".date-picker").datepicker({
		language: "ko",
		todayButton: true,
		clearButton: true,
		onSelect: function ()
		{
			datepicker.hide();
		}
	}).data('datepicker');
	$(".search-option").on('hide.bs.dropdown', function (e)
	{
		if (e.clickEvent) e.preventDefault();
	});
	$("#product-keyword").val(`${$("#keyword").val()}`);
	$("#product-confirmed").prop("checked", $("#confirmed").val());
	if (date) datepicker.selectDate(new Date(parseInt(date)));

	FilterProduct();
}

$(document).ready(function ()
{
	$("#category-select").on("change", function ()
	{
		FilterProduct();
	});
	$("#sorting-select").on("change", function ()
	{
		FilterProduct();
	});
	$("#product-keyword").on("keyup", function ()
	{
		FilterProduct();
	});
	$("#option-apply").on("click", function ()
	{
		$("#search-option-dropdown").dropdown("toggle");
		FilterProduct();
	});
	$("#option-reset").on("click", function ()
	{
		appointment.update({
			from: min,
			to: max
		});
		datepicker.clear();
		$("#product-confirmed").prop("checked", false);

		$("#search-option-dropdown").dropdown("toggle");
		FilterProduct();
	});

	Initialize();
});