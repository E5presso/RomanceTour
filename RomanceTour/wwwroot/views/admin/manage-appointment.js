var min = 0;
var max = 500;
var step = 10;
var datepicker;

var appointment;
var fromAppointment = null;
var toAppointment = null;

var paid;
var fromPaid = null;
var toPaid = null;

var productKeyword = "";
var appointmentKeyword = "";

var selectedProduct = null;
var selectedSession = null;
var selectedAppointment = null;

var sessionStatus = {
	"AVAILABLE": 0,
	"APPROVED": 1,
	"FULLED": 2,
	"CANCELED": 3
};

function ListProduct()
{
	var category = $("#product-category option:selected").val();
	var keyword = $("#product-keyword").val();

	if (category == "confirmed")
	{
		AjaxWithoutLoading("/Product/FilterProduct", {
			category: 0,
			filter: {
				Keyword: keyword,
				Confirmed: true
			}
		}, ListProductCallback);
	}
	else
	{
		AjaxWithoutLoading("/Product/FilterProduct", {
			category: parseInt(category),
			filter: {
				Keyword: keyword,
				Confirmed: false
			}
		}, ListProductCallback);
	}
}
function ListSession()
{
	var id = selectedProduct;
	var status = $("#session-status option:selected").val();
	AjaxWithoutLoading("/Appointment/FilterSession", {
		id: id,
		status: status,
		filter: {
			FromAppointment: fromAppointment,
			ToAppointment: toAppointment,
			FromPaid: fromPaid,
			ToPaid: toPaid,
			FromDate: datepicker.selectedDates[0] ? datepicker.selectedDates[0].toISOString() : null,
			ToDate: datepicker.selectedDates[1] ? datepicker.selectedDates[1].toISOString() : null
		}
	}, ListSessionCallback);
}
function GetSession()
{
	AjaxWithoutLoading("/Appointment/GetSession", {
		id: selectedSession
	}, GetSessionCallback);
}
function UpdateSession()
{
	var status = parseInt($("#session-input-status").val());
	Ajax("/Appointment/UpdateSession", {
		id: selectedSession,
		status: status
	}, UpdateSessionCallback);
}
function CountDeparture()
{
	var departure = parseInt($("#session-input-departure option:selected").val());
	AjaxWithoutLoading("/Appointment/CountDeparture", {
		id: selectedSession,
		departureId: departure
	}, CountDepartureCallback);
}
function CountPriceRule()
{
	var priceRule = parseInt($("#session-input-rule option:selected").val());
	AjaxWithoutLoading("/Appointment/CountPriceRule", {
		id: selectedSession,
		priceRuleId: priceRule
	}, CountPriceRuleCallback);
}
function ListAppointment()
{
	var status = parseInt($("#appointment-status").val());
	var keyword = $("#appointment-keyword").val();
	AjaxWithoutLoading("/Appointment/FilterAppointment", {
		id: selectedSession,
		status: status,
		keyword: keyword
	}, ListAppointmentCallback);
}
function GetAppointment()
{
	AjaxWithoutLoading("/Appointment/GetAppointmentStatus", {
		id: selectedAppointment
	}, GetAppointmentCallback);
}
function UpdateAppointment()
{
	var status = parseInt($("#appointment-input-status").val());
	Ajax("/Appointment/UpdateAppointmentStatus", {
		id: selectedAppointment,
		status: status
	}, UpdateAppointmentCallback);
}

function ListProductCallback(model)
{
	$("#product-list tbody").html("");
	$.each(model, function (index, item)
	{
		var row = `
			<tr class="product-item" id="product-${item.Id}">
				<td>
					<span class="table-body">
						<img class="thumbnail-image" src="${item.Thumbnail}" />
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Title}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.SubTitle}
					</span>
				</td>
			</tr>
		`;
		if (productKeyword) row = row.replaceAll(productKeyword, `<span class="highlighted">${productKeyword}</span>`);
		$("#product-list tbody").append(row);
		$(`#product-${item.Id}`).on("mouseover", function ()
		{
			$(this).addClass("hovered");
		});
		$(`#product-${item.Id}`).on("mouseout", function ()
		{
			$(this).removeClass("hovered");
		});
		$(`#product-${item.Id}`).on("click", function ()
		{
			$(".product-item").removeClass("selected");
			$(this).addClass("selected");
			selectedProduct = item.Id;
			ListProduct();
			ListSession();
		});
	});
	$(`#product-${selectedProduct}`).addClass("selected");
}
function ListSessionCallback(model)
{
	$("#session-list tbody").html("");
	$.each(model, function (index, item)
	{
		$("#session-list tbody").append(`
			<tr class="session-item" id="session-${item.Id}">
				<td>
					<span class="table-body">
						${new Date(item.Date).format(`yyyy년 MM월 dd일`)}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Reserved}명
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Paid}명
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Status == "AVAILABLE" ?
				"예약가능" : item.Status == "APPROVED" ?
					"출발확정" : item.Status == "FULLED" ?
						"예약마감" : item.Status == "CANCELED" ?
							"예약취소" : "오류"}
					</span>
				</td>
			</tr>
		`);
		$(`#session-${item.Id}`).on("mouseover", function ()
		{
			$(this).addClass("hovered");
		});
		$(`#session-${item.Id}`).on("mouseout", function ()
		{
			$(this).removeClass("hovered");
		});
		$(`#session-${item.Id}`).on("click", function ()
		{
			$(".session-item").removeClass("selected");
			$(this).addClass("selected");
			selectedSession = item.Id;
			ListProduct();
			ListSession();
			GetSession();
			ListAppointment();
		});
	});
	$(`#session-${selectedSession}`).addClass("selected");
}
function GetSessionCallback(model)
{
	$("#session-info-date").prop("disabled", false);
	$("#session-info-appointment").prop("disabled", false);
	$("#session-info-paid").prop("disabled", false);
	$("#session-input-group").prop("disabled", false);
	$("#session-info-bus").prop("disabled", false);
	$("#session-input-departure").prop("disabled", false);
	$("#session-info-departure").prop("disabled", false);
	$("#session-input-rule").prop("disabled", false);
	$("#session-info-rule").prop("disabled", false);
	$("#session-info-sales").prop("disabled", false);
	$("#session-input-status").prop("disabled", false);
	$("#session-save").prop("disabled", false);

	$("#session-info-date").val(new Date(model.Date).format("yyyy년 MM월 dd일"));
	$("#session-info-appointment").val(`${model.Reserved}명`);
	$("#session-info-paid").val(`${model.Paid}명`);
	$("#session-info-sales").val($("<div />").html(`&#8361;${model.Sales.format()}`).text());
	var group = parseInt($("#session-input-group").val());
	if (!isNaN(group) && group != 0)
	{
		var paid = parseInt($("#session-info-paid").val().replace("명", ""));
		var bus = Math.ceil(paid / group);
		$("#session-info-bus").val(`${bus} 대`);
	}
	else $("#session-info-bus").val(`- 대`);

	CountDeparture();
	CountPriceRule();

	console.log(sessionStatus[model.Status]);
	$(`#session-input-status option`).attr("selected", false);
	$(`#session-input-status`).val(sessionStatus[model.Status]);
}
function UpdateSessionCallback(_)
{
	ListProduct();
	ListSession();
	GetSession();
	ListAppointment();
}
function CountDepartureCallback(model)
{
	$("#session-info-departure").val(`${model}명`);
}
function CountPriceRuleCallback(model)
{
	$("#session-info-rule").val(`${model}명`);
}
function ListAppointmentCallback(model)
{
	$("#appointment-list tbody").html("");
	$.each(model, function (index, item)
	{
		var row = `
			<tr class="appointment-item" id="appointment-${item.Id}">
				<td>
					<span class="table-body">
						#${item.Id}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Name}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Ammount}명
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.IsUser ? "회원" : "비회원"}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Status == "READY_TO_PAY" ? "입금대기" : item.Status == "CONFIRMED" ? "입금완료" : item.Status == "CANCELED" ? "예약취소" : "오류"}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Phone}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Address}
					</span>
				</td>
				<td>
					<span class="table-body">
						&#8361;${item.Price.format()}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.BillingName}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.BillingBank}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.BillingNumber}
					</span>
				</td>
			</tr>
		`;
		if (appointmentKeyword) row = row.replaceAll(appointmentKeyword, `<span class="highlighted">${appointmentKeyword}</span>`);
		$("#appointment-list tbody").append(row);
		$(`#appointment-${item.Id}`).on("mouseover", function ()
		{
			$(this).addClass("hovered");
		});
		$(`#appointment-${item.Id}`).on("mouseout", function ()
		{
			$(this).removeClass("hovered");
		});
		$(`#appointment-${item.Id}`).on("click", function ()
		{
			$(".appointment-item").removeClass("selected");
			$(this).addClass("selected");
			selectedAppointment = item.Id;
			ListProduct();
			ListSession();
			GetSession();
			ListAppointment();
			GetAppointment();
		});
	});
	$(`#appointment-${selectedAppointment}`).addClass("selected");
}
function GetAppointmentCallback(model)
{
	$("#appointment-input-status").prop("disabled", false);
	$(`#appointment-input-status option`).attr("selected", false);
	$(`#appointment-input-status`).val(model);
	$("#appointment-save").prop("disabled", false);
}
function UpdateAppointmentCallback(_)
{
	ListProduct();
	ListSession();
	GetSession();
	ListAppointment();
	GetAppointment();
}

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
function PaidUpdateCallback(data)
{
	fromPaid = data.from;
	toPaid = data.to;
}
function PaidChangeCallback(data)
{
	fromPaid = data.from;
	toPaid = data.to;
}

function Initialize()
{
	$(".go-back").on("click", function ()
	{
		window.location.href = "/Admin/Dashboard";
	});
	$("#appointment-range").ionRangeSlider({
		type: "double",
		min: min,
		max: max,
		step: step,
		prettify_enabled: true,
		prettify_separator: ",",
		postfix: "명",
		values_separator: " ~ ",
		onChange: AppointmentChangeCallback,
		onUpdate: AppointmentUpdateCallback
	});
	$(".search-bar-second .search-text").on("focus", function ()
	{
		$(this).parent().css("box-shadow", "0px 0px 10px #FFC3BD");
		$(this).parent().css("border", "1px solid #FFC3BD");
		$(this).parent().find(".user-search-btn > .fa-search").css({
			"color": "#FFC3BD",
			"text-shadow": "0px 0px 10px #FFC3BD"
		});
	});
	$(".search-bar-second .search-text").on("focusout", function ()
	{
		$(this).parent().css("box-shadow", "2px 2px 5px rgba(18, 18, 18, 0.3)");
		$(this).parent().css("border", "1px solid #FFC3BD");
		$(this).parent().find(".user-search-btn > .fa-search").css({
			"color": "#FFC3BD",
			"text-shadow": "none"
		});
	});

	appointment = $("#appointment-range").data("ionRangeSlider");
	$("#paid-range").ionRangeSlider({
		type: "double",
		min: min,
		max: max,
		step: step,
		prettify_enabled: true,
		prettify_separator: ",",
		postfix: "명",
		values_separator: " ~ ",
		onChange: PaidChangeCallback,
		onUpdate: PaidUpdateCallback
	});
	paid = $("#paid-range").data("ionRangeSlider");
	datepicker = $(".date-picker").datepicker({
		language: "ko",
		todayButton: true,
		clearButton: true,
		toggleSelected: false,
		autoClose: true,
		range: true
	}).data('datepicker');

	$(".search-option").on('hide.bs.dropdown', function (e)
	{
		if (e.clickEvent) e.preventDefault();
	});

	$("#option-apply").on("click", function ()
	{
		$("#search-option-dropdown").dropdown("toggle");
		ListSession();
	});
	$("#option-reset").on("click", function ()
	{
		appointment.update({
			from: min,
			to: max
		});
		paid.update({
			from: min,
			to: max
		});
		datepicker.clear();
		$("#search-option-dropdown").dropdown("toggle");
		ListSession();
	});

	$("#product-category").on("change", function ()
	{
		ListProduct();
	});
	$("#product-keyword").on("keyup", function ()
	{
		productKeyword = $("#product-keyword").val();
		ListProduct();
	});

	$("#session-status").on("change", function ()
	{
		ListSession();
	});

	$("#session-input-group").on("keyup", function ()
	{
		var group = parseInt($("#session-input-group").val());
		if (!isNaN(group) && group != 0)
		{
			var paid = parseInt($("#session-info-paid").val().replace("명", ""));
			var bus = Math.ceil(paid / group);
			$("#session-info-bus").val(`${bus} 대`);
		}
		else $("#session-info-bus").val(`- 대`);
	});
	$("#session-input-departure").on("change", function ()
	{
		CountDeparture();
	});
	$("#session-input-rule").on("change", function ()
	{
		CountPriceRule();
	});
	$("#session-save").on("click", function ()
	{
		UpdateSession();
	});

	$("#appointment-status").on("change", function ()
	{
		ListAppointment();
	});
	$("#appointment-keyword").on("keyup", function ()
	{
		appointmentKeyword = $("#appointment-keyword").val();
		ListAppointment();
	});
	$("#appointment-save").on("click", function ()
	{
		UpdateAppointment();
	});
}

$(document).ready(function ()
{
	Initialize();
	ListProduct();
});