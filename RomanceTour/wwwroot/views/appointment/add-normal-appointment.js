var datepicker;
var isPopupOpened = false;
var personForm;
var personCount = 1;
var back;
var dates = [];
var DateSessionStatus = {
	AVAILABLE: "AVAILABLE",
	APPROVED: "APPROVED",
	FULLED: "FULLED",
	CANCELED: "CANCELED"
};
var PriceRuleType = {
	PERCENT_AS: "PERCENT_AS",
	PERCENT_PLUS: "PERCENT_PLUS",
	PERCENT_MINUS: "PERCENT_MINUS",
	STATIC_PLUS: "STATIC_PLUS",
	STATIC_MINUS: "STATIC_MINUS"
};

function ValidateName(name) {
	var regex = /^[가-힣]{2,4}$/;
	return regex.test(name);
}
function ValidatePhone(phone) {
	var regex = /(01[016789])([1-9]{1}[0-9]{2,3})([0-9]{4})$/;
	return regex.test(phone);
}
function ValidatePassword(passwd) {
	var regex = /^(?=.*[a-zA-Z])(?=.*[0-9]).{4,20}$/;
	return regex.test(passwd);
}
function ValidateForm() {
	setTimeout(function () {
		if ($(".date-picker").hasClass("is-valid") &&
			$("#address").hasClass("is-valid") &&
			$("#phone").hasClass("is-valid") &&
			$("#name").hasClass("is-valid") &&
			$("#password").hasClass("is-valid") &&
			$("#confirm-password").hasClass("is-valid") &&
			$("#billing-name").hasClass("is-valid"))
			$("#appointment").prop("disabled", false);
		else $("#appointment").prop("disabled", true);
	}, 100);
}

function CalculatePrice() {
	var totalPrice = 0;
	$(".person").each(function (index, item) {
		var ageValue = $(item).find(".age").children("option:selected").val();
		var ageParsed = ageValue.split("-");
		var agePrice = parseInt(ageParsed[2]);
		var price = agePrice;

		var optionValue = $(item).find(".option").children("option:selected");
		$.each(optionValue, function (index, item) {
			var optionParsed = $(item).val().split("-");
			var optionRuleType = parseInt(optionParsed[1]);
			var optionPrice = parseInt(optionParsed[2]);
			switch (optionRuleType)
			{
				case PriceRuleType.STATIC_MINUS: {
					price -= optionPrice;
					break;
				}
				case PriceRuleType.STATIC_PLUS: {
					price += optionPrice;
					break;
				}
			}
		});

		var person = parseInt($(item).find(".ammount").val());

		totalPrice += (price * person);
	});
	$("#price-value").html(`&#8361;${totalPrice.format()}`);
}
function Initialize() {
	var header = parseFloat($("header").height()) + parseFloat($("header").css("paddingTop")) * 2;
	var collapse = $(".collapse").css("display") == "none" || $(window).width() >= 1200 ? 0 : parseFloat($(".collapse").height() + parseFloat($(".collapse").css("paddingTop")) * 2);
	var padding = parseFloat($(".full-screen-area").css("paddingTop"));
	var id = parseInt($("#product-id").val());

	$("article").height(window.innerHeight - header + collapse - (padding * 2));
	$(window).resize(function () {
		var header = parseFloat($("header").height()) + parseFloat($("header").css("paddingTop")) * 2;
		var collapse = $(".collapse").css("display") == "none" || $(window).width() >= 1200 ? 0 : parseFloat($(".collapse").height() + parseFloat($(".collapse").css("paddingTop")) * 2);
		var padding = parseFloat($(".full-screen-area").css("paddingTop"));

		$("article").height(window.innerHeight - header + collapse - (padding * 2));
	});
	personForm = $("#people").html();

	$(".person").css({
		display: "block"
	});
	$(".multiple-select").selectpicker({
		noneSelectedText: "선택된 옵션이 없습니다.",
		countSelectedText: "{0}개의 옵션 선택됨"
	});
	$(".input-spinner").inputSpinner();
	back = $("#back").val();

	Ajax("/Appointment/GetAvailable", {
		id: id
	}, GetAvailableCallback);
	CalculatePrice();
}

function GetAvailableCallback(model) {
	$.each(model, function (index, item) {
		var date = new Date(item.Date);
		var status = item.Status;
		dates.push({
			date: date,
			status: status
		});
	});
	datepicker = $(".date-picker").datepicker({
		language: "ko",
		todayButton: true,
		clearButton: true,
		autoClose: true,
		onRenderCell: function (date, cellType) {
			switch (cellType)
			{
				case "year": {
					var currentYear = date.getFullYear();
					if (dates.find(x => x.date.getFullYear() == date.getFullYear()))
					{
						return {
							disabled: false,
							html:
								`<span class="date-cell date-normal">`
								+ `<span class="date-cell-day">${currentYear}</span>`
								+ `</span>`
						}
					}
					else
					{
						return {
							disabled: true,
							html:
								`<span class="date-cell date-cell-disabled">`
								+ `<span class="date-cell-day">${currentYear}</span>`
								+ `</span>`
						}
					}
				}
				case "month": {
					var currentYear = date.getFullYear();
					var currentMonth = date.getMonth() + 1;
					if (dates.find(x => x.date.getMonth() == date.getMonth()))
					{
						return {
							disabled: false,
							html:
								`<span class="date-cell date-normal">`
								+ `<span class="date-cell-day">${currentMonth}월</span>`
								+ `</span>`
						}
					}
					else
					{
						return {
							disabled: true,
							html:
								`<span class="date-cell date-cell-disabled">`
								+ `<span class="date-cell-day">${currentMonth}월</span>`
								+ `</span>`
						}
					}
				}
				case "day": {
					var currentDate = date.getDate();
					var matched = dates.find(x => x.date.getFullYear() == date.getFullYear()
						&& x.date.getMonth() == date.getMonth()
						&& x.date.getDate() == date.getDate());
					if (matched)
					{
						switch (matched.status)
						{
							case DateSessionStatus.AVAILABLE: {
								return {
									disabled: false,
									html:
										`<span class="date-cell date-available">`
										+ `<span class="date-cell-day">${currentDate}</span>`
										+ `</span>`
								}
							}
							case DateSessionStatus.APPROVED: {
								return {
									disabled: false,
									html:
										`<span class="date-cell date-approved">`
										+ `<span class="date-cell-day">${currentDate}</span>`
										+ `</span>`
								}
							}
							case DateSessionStatus.FULLED: {
								return {
									disabled: true,
									html:
										`<span class="date-cell date-fulled">`
										+ `<span class="date-cell-day">${currentDate}</span>`
										+ `</span>`
								}
							}
							case DateSessionStatus.CANCELED: {
								return {
									disabled: true,
									html:
										`<span class="date-cell date-canceled">`
										+ `<span class="date-cell-day">${currentDate}</span>`
										+ `</span>`
								}
							}
						}
					}
					else return {
						disabled: true,
						html:
							`<span class="date-cell date-cell-disabled">`
							+ `<span class="date-cell-day">${currentDate}</span>`
							+ `</span>`
					}
				}
			}
		},
		onSelect: function (fomattedDate, date, inst) {
			if (datepicker.selectedDates.length)
			{
				$(".date-picker").siblings("label").html("날짜가 선택되었습니다.");
				$(".date-picker").siblings("label").removeClass("text-danger");
				$(".date-picker").siblings("label").addClass("text-success");
				$(".date-picker").removeClass("is-invalid");
				$(".date-picker").addClass("is-valid");
			}
			else
			{
				$(".date-picker").siblings("label").html("날짜를 선택해주세요.");
				$(".date-picker").siblings("label").removeClass("text-success");
				$(".date-picker").siblings("label").addClass("text-danger");
				$(".date-picker").removeClass("is-valid");
				$(".date-picker").addClass("is-invalid");
			}
			ValidateForm();
		}
	}).data('datepicker');
}
function AddCallback(model) {
	alert(model.Message);
	if (model.Result) window.location.href = `/Product/GetProduct?id=${$("#product-id").val()}`;
}

$(document).ready(function () {
	Initialize();

	$("#name").on("keyup", function () {
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
		ValidateForm();
	});
	$("#phone").on("keyup", function () {
		var value = $("#phone").val();
		if (value.length > 0)
		{
			if (ValidatePhone(value))
			{
				$("#phone").siblings("label").html("유효한 번호입니다.");
				$("#phone").siblings("label").removeClass("text-danger");
				$("#phone").siblings("label").addClass("text-success");
				$("#phone").removeClass("is-invalid");
				$("#phone").addClass("is-valid");
			}
			else
			{
				$("#phone").siblings("label").html("유효하지 않은 번호입니다.");
				$("#phone").siblings("label").removeClass("text-success");
				$("#phone").siblings("label").addClass("text-danger");
				$("#phone").removeClass("is-valid");
				$("#phone").addClass("is-invalid");
			}
		}
		else
		{
			$("#phone").removeClass("is-invalid");
			$("#phone").removeClass("is-valid");
			$("#phone").siblings("label").html("휴대폰 번호");
			$("#phone").siblings("label").removeClass("text-danger");
			$("#phone").siblings("label").removeClass("text-success");
		}
		ValidateForm();
	});
	$("#address").on("focus", function () {
		if (!isPopupOpened)
		{
			new daum.Postcode({
				oncomplete: function (result) {
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
					ValidateForm();
				},
				onclose: function () {
					isPopupOpened = false;
					$("#billing-name").focus();
				}
			}).open();
			isPopupOpened = true;
		}
	});
	$("#password").on("keyup", function () {
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
			$("#password").siblings("label").html("비밀번호 (예약 확인 시 필요합니다.)");
			$("#password").siblings("label").removeClass("text-danger");
			$("#password").siblings("label").removeClass("text-success");
			$("#password").removeClass("is-invalid");
			$("#password").removeClass("is-valid");
		}
		ValidateForm();
		$("#confirm-password").trigger("keyup");
	});
	$("#confirm-password").on("keyup", function () {
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
		ValidateForm();
	});

	$("#billing-name").on("keyup", function () {
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
		ValidateForm();
	});
	$(".age").on("change", function (e) {
		e.stopPropagation();
		CalculatePrice();
	});
	$(".option").on("change", function (e) {
		e.stopPropagation();
		CalculatePrice();
	});
	$(".ammount").on("change", function (e) {
		if ($(this).val().length == 0) $(this).val(1);
		e.stopPropagation();
		CalculatePrice();
	});
	$(".delete-person").on("click", function () {
		if ($(".person").length > 1)
		{
			var parent = $(this).parent().parent().parent();
			parent.slideUp(DURATION, function () {
				parent.remove();
				CalculatePrice();
			});
		}
		else alert("예약인원은 비어있을 수 없습니다.");
	});
	$("#add-person").on("click", function () {
		$("#people").append(personForm);
		$(".person:last-child()").attr("id", `person-${personCount}`);
		$(`#person-${personCount}`).slideDown(DURATION);
		$(`#person-${personCount}`).find(".age").on("change", function (e) {
			e.stopPropagation();
			CalculatePrice();
		});
		$(`#person-${personCount}`).find(".option").on("change", function (e) {
			e.stopPropagation();
			CalculatePrice();
		});
		$(`#person-${personCount}`).find(".ammount").on("change", function (e) {
			if ($(this).val().length == 0) $(this).val(1);
			e.stopPropagation();
			CalculatePrice();
		});
		$(`#person-${personCount}`).find(".delete-person").on("click", function () {
			if ($(".person").length > 1)
			{
				var parent = $(this).parent().parent().parent();
				parent.slideUp(DURATION, function () {
					parent.remove();
					CalculatePrice();
				});
			}
			else alert("예약인원은 비어있을 수 없습니다.");
		});
		$(`#person-${personCount}`).find(".multiple-select").selectpicker({
			noneSelectedText: "선택된 옵션이 없습니다.",
			countSelectedText: "{0}개의 옵션 선택됨"
		});
		$(`#person-${personCount}`).find(".input-spinner").inputSpinner();
		CalculatePrice();
		personCount++;
	});
	$("#appointment").on("click", function () {
		var appointment = new FormData();
		appointment.append("Id", parseInt($("#product-id").val()));
		appointment.append("IsUserAppointment", false);
		appointment.append("DateString", datepicker.selectedDates[0].toDotNetDateTime());
		appointment.append("Name", $("#name").val());
		appointment.append("Phone", $("#phone").val());
		appointment.append("Address", $("#address").val());
		appointment.append("Password", $("#password").val());
		appointment.append("BillingName", $("#billing-name").val());
		appointment.append("BillingBank", $("#billing-bank option:selected").val());
		appointment.append("BillingNumber", $("#billing-number").val());
		$(".person").each(function (index1, item1) {
			appointment.append(`People[${index1}].Price`, parseInt($(item1).find(".age option:selected").val().split('-')[0]));
			appointment.append(`People[${index1}].Departure`, parseInt($(item1).find(".departure option:selected").val().split('-')[0]));
			$(item1).find(".option option:selected").each(function (index2, item2) {
				appointment.append(`People[${index1}].Options[${index2}]`, parseInt($(item2).val().split('-')[0]));
			});
			appointment.append(`People[${index1}].Ammount`, parseInt($(item1).find(".ammount").val()));
		});

		AjaxForm("/Appointment/AddAppointment", appointment, AddCallback);
	});
	$("#cancel").on("click", function () {
		if (confirm("예약을 취소할 경우 입력한 내용이 지워집니다.\n정말로 취소하시겠습니까?")) window.location.href = back;
	});
});