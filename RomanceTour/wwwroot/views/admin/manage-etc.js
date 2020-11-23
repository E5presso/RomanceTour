var categoryKeyword = "";
var billingKeyword = "";
var departureKeyword = "";
var priceRuleKeyword = "";
var hostKeyword = "";

var isPopupOpened = false;
var PriceRuleType =
{
	PERCENT_AS: "PERCENT_AS",
	PERCENT_PLUS: "PERCENT_PLUS",
	PERCENT_MINUS: "PERCENT_MINUS",
	STATIC_PLUS: "STATIC_PLUS",
	STATIC_MINUS: "STATIC_MINUS"
};

function ListCategory()
{
	Ajax("/Etc/ListCategory", {}, ListCategoryCallback);
}
function ListBilling()
{
	Ajax("/Etc/ListBilling", {}, ListBillingCallback);
}
function ListDeparture()
{
	Ajax("/Etc/ListDeparture", {}, ListDepartureCallback);
}
function ListPriceRule()
{
	Ajax("/Etc/ListPriceRule", {}, ListPriceRuleCallback);
}
function ListHost()
{
	Ajax("/Etc/ListHost", {}, ListHostCallback);
}

function FilterCategory()
{
	categoryKeyword = $("#category-keyword").val();
	AjaxWithoutLoading("/Etc/FilterCategory", {
		keyword: categoryKeyword
	}, ListCategoryCallback);
}
function FilterBilling()
{
	billingKeyword = $("#billing-keyword").val();
	AjaxWithoutLoading("/Etc/FilterBilling", {
		keyword: billingKeyword
	}, ListBillingCallback);
}
function FilterDeparture()
{
	departureKeyword = $("#departure-keyword").val();
	AjaxWithoutLoading("/Etc/FilterDeparture", {
		keyword: departureKeyword
	}, ListDepartureCallback);
}
function FilterPriceRule()
{
	priceRuleKeyword = $("#price-rule-keyword").val();
	AjaxWithoutLoading("/Etc/FilterPriceRule", {
		keyword: priceRuleKeyword
	}, ListPriceRuleCallback);
}
function FilterHost()
{
	hostKeyword = $("#host-keyword").val();
	AjaxWithoutLoading("/Etc/FilterHost", {
		keyword: hostKeyword
	}, ListHostCallback);
}

function AddCategoryCallback(_)
{
	HidePopup($("#category-popup"));
	location.reload();
}
function AddBillingCallback(_)
{
	HidePopup($("#billing-popup"));
	ListBilling();
}
function AddDepartureCallback(_)
{
	HidePopup($("#departure-popup"));
	ListDeparture();
}
function AddPriceRuleCallback(_)
{
	HidePopup($("#price-rule-popup"));
	ListPriceRule();
}
function AddHostCallback(_)
{
	HidePopup($("#host-popup"));
	ListHost();
}

function ListCategoryCallback(model)
{
	$("#category-list").html('');
	$.each(model, function (_, item)
	{
		var row = `
			<tr id="category-${item.Id}">
				<td>
					<span class="table-body">
						${item.Name}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Description}
					</span>
				</td>
				<td>
					<span class="table-body">
						<button class="btn third config-btn">
							<span class="fa fa-cog btn-icon"></span>
						</button>
						<button class="btn third remove-btn">
							<span class="fa fa-trash btn-icon"></span>
						</button>
					</span>
				</td>
			</tr>
		`;
		if (categoryKeyword) row = row.replaceAll(categoryKeyword, `<span class="highlighted">${categoryKeyword}</span>`);
		$("#category-list").append(row);
		$(`#category-${item.Id} .config-btn`).on("click", function ()
		{
			var id = parseInt($(this).parent().parent().parent().attr("id").split("-")[1]);
			Ajax("/Etc/GetCategory", {
				id: id
			}, GetCategoryCallback);
		});
		$(`#category-${item.Id} .remove-btn`).on("click", function ()
		{
			if (confirm("정말로 해당 카테고리를 제거하시겠습니까?\n이 동작은 취소할 수 없습니다."))
			{
				var id = parseInt($(this).parent().parent().parent().attr("id").split("-")[1]);
				Ajax("/Etc/RemoveCategory", {
					id: id
				}, RemoveCategoryCallback);
			}
		});
	});
}
function ListBillingCallback(model)
{
	$("#billing-list").html('');
	$.each(model, function (_, item)
	{
		var row = `
			<tr id="billing-${item.Id}">
				<td>
					<span class="table-body">
						${item.Name}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Bank}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Number}
					</span>
				</td>
				<td>
					<span class="table-body">
						<button class="btn third config-btn">
							<span class="fa fa-cog btn-icon"></span>
						</button>
						<button class="btn third remove-btn">
							<span class="fa fa-trash btn-icon"></span>
						</button>
					</span>
				</td>
			</tr>
		`;
		if (billingKeyword) row = row.replaceAll(billingKeyword, `<span class="highlighted">${billingKeyword}</span>`);
		$("#billing-list").append(row);
		$(`#billing-${item.Id} .config-btn`).on("click", function ()
		{
			var id = parseInt($(this).parent().parent().parent().attr("id").split("-")[1]);
			Ajax("/Etc/GetBilling", {
				id: id
			}, GetBillingCallback);
		});
		$(`#billing-${item.Id} .remove-btn`).on("click", function ()
		{
			if (confirm("정말로 해당 입금계좌를 제거하시겠습니까?\n이 동작은 취소할 수 없습니다."))
			{
				var id = parseInt($(this).parent().parent().parent().attr("id").split("-")[1]);
				Ajax("/Etc/RemoveBilling", {
					id: id
				}, RemoveBillingCallback);
			}
		});
	});
}
function ListDepartureCallback(model)
{
	$("#departure-list").html('');
	$.each(model, function (_, item)
	{
		var row = `
			<tr id="departure-${item.Id}">
				<td>
					<span class="table-body">
						${item.Name}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Latitude}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Longitude}
					</span>
				</td>
				<td>
					<span class="table-body">
						<button class="btn third config-btn">
							<span class="fa fa-cog btn-icon"></span>
						</button>
						<button class="btn third remove-btn">
							<span class="fa fa-trash btn-icon"></span>
						</button>
					</span>
				</td>
			</tr>
		`;
		if (departureKeyword) row = row.replaceAll(departureKeyword, `<span class="highlighted">${departureKeyword}</span>`);
		$("#departure-list").append(row);
		$(`#departure-${item.Id} .config-btn`).on("click", function ()
		{
			var id = parseInt($(this).parent().parent().parent().attr("id").split("-")[1]);
			Ajax("/Etc/GetDeparture", {
				id: id
			}, GetDepartureCallback);
		});
		$(`#departure-${item.Id} .remove-btn`).on("click", function ()
		{
			if (confirm("정말로 해당 탑승지를 제거하시겠습니까?\n이 동작은 취소할 수 없습니다."))
			{
				var id = parseInt($(this).parent().parent().parent().attr("id").split("-")[1]);
				Ajax("/Etc/RemoveDeparture", {
					id: id
				}, RemoveDepartureCallback);
			}
		});
	});
}
function ListPriceRuleCallback(model)
{
	$("#price-rule-list").html('');
	$.each(model, function (_, item)
	{
		var row = `
			<tr id="price-rule-${item.Id}">
				<td>
					<span class="table-body">
						${item.RuleType == PriceRuleType.PERCENT_AS ? "요금 옵션" :
							item.RuleType == PriceRuleType.PERCENT_PLUS ? "유동가 추가옵션" :
							item.RuleType == PriceRuleType.PERCENT_MINUS ? "유동가 할인옵션" :
							item.RuleType == PriceRuleType.STATIC_PLUS ? "고정가 추가옵션" :
							item.RuleType == PriceRuleType.STATIC_MINUS ? "고정가 할인옵션" :
							"?????"
						}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.RuleName}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Description}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Price}
					</span>
				</td>
				<td>
					<span class="table-body">
						<button class="btn third config-btn">
							<span class="fa fa-cog btn-icon"></span>
						</button>
						<button class="btn third remove-btn">
							<span class="fa fa-trash btn-icon"></span>
						</button>
					</span>
				</td>
			</tr>
		`;
		if (priceRuleKeyword) row = row.replaceAll(priceRuleKeyword, `<span class="highlighted">${priceRuleKeyword}</span>`);
		$("#price-rule-list").append(row);
		$(`#price-rule-${item.Id} .config-btn`).on("click", function ()
		{
			var id = parseInt($(this).parent().parent().parent().attr("id").split("-")[2]);
			Ajax("/Etc/GetPriceRule", {
				id: id
			}, GetPriceRuleCallback);
		});
		$(`#price-rule-${item.Id} .remove-btn`).on("click", function ()
		{
			if (confirm("정말로 해당 가격정책을 제거하시겠습니까?\n이 동작은 취소할 수 없습니다."))
			{
				var id = parseInt($(this).parent().parent().parent().attr("id").split("-")[2]);
				Ajax("/Etc/RemovePriceRule", {
					id: id
				}, RemovePriceRuleCallback);
			}
		});
	});
}
function ListHostCallback(model)
{
	$("#host-list").html('');
	$.each(model, function (_, item)
	{
		var row = `
			<tr id="host-${item.Id}">
				<td>
					<span class="table-body">
						${item.Type == 0 ? "숙소" : item.Type == 1 ? "식당" : ""}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Name}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.Address}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.HostName}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.HostPhone}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.HostBank}
					</span>
				</td>
				<td>
					<span class="table-body">
						${item.HostBillingNumber}
					</span>
				</td>
				<td>
					<span class="table-body">
						<button class="btn third config-btn">
							<span class="fa fa-cog btn-icon"></span>
						</button>
						<button class="btn third remove-btn">
							<span class="fa fa-trash btn-icon"></span>
						</button>
					</span>
				</td>
			</tr>
		`;
		if (hostKeyword) row = row.replaceAll(hostKeyword, `<span class="highlighted">${hostKeyword}</span>`);
		$("#host-list").append(row);
		$(`#host-${item.Id} .config-btn`).on("click", function ()
		{
			var id = parseInt($(this).parent().parent().parent().attr("id").split("-")[1]);
			Ajax("/Etc/GetHost", {
				id: id
			}, GetHostCallback);
		});
		$(`#host-${item.Id} .remove-btn`).on("click", function ()
		{
			if (confirm("정말로 해당 제휴업체를 제거하시겠습니까?\n이 동작은 취소할 수 없습니다."))
			{
				var id = parseInt($(this).parent().parent().parent().attr("id").split("-")[1]);
				Ajax("/Etc/RemoveHost", {
					id: id
				}, RemoveHostCallback);
			}
		});
	});
}

function GetCategoryCallback(model)
{
	if (model.Result)
	{
		var data = model.Data;
		$("#category-id").val(data.Id);
		$("#category-name").val(data.Name);
		$("#category-description").val(data.Description);
		ShowPopup($("#category-popup"));
	}
	else alert("존재하지 않는 카테고리입니다.");
}
function GetBillingCallback(model)
{
	if (model.Result)
	{
		var data = model.Data;
		$("#billing-id").val(data.Id);
		$("#billing-name").val(data.Name);
		$("#billing-bank").val(data.Bank);
		$("#billing-number").val(data.Number);
		ShowPopup($("#billing-popup"));
	}
	else alert("존재하지 않는 입금계좌입니다.");
}
function GetDepartureCallback(model)
{
	if (model.Result)
	{
		var data = model.Data;
		$("#departure-id").val(data.Id);
		$("#departure-name").val(data.Name);
		$("#departure-latitude").val(data.Latitude);
		$("#departure-longitude").val(data.Longitude);
		ShowPopup($("#departure-popup"));
	}
	else alert("존재하지 않는 탑승지입니다.");
}
function GetPriceRuleCallback(model)
{
	if (model.Result)
	{
		var data = model.Data;
		$("#price-rule-id").val(data.Id);
		$("#popup-price-rule-type").val(data.RuleType);
		$("#price-rule-name").val(data.RuleName);
		$("#price-rule-description").val(data.Description);
		$("#price-rule-price").val(data.Price);
		ShowPopup($("#price-rule-popup"));
	}
	else alert("존재하지 않는 가격정책입니다.");
}
function GetHostCallback(model)
{
	if (model.Result)
	{
		var data = model.Data;
		$("#host-id").val(data.Id);
		$("#popup-host-type").val(data.Type);
		$("#host-name").val(data.Name);
		$("#host-address").val(data.Address);
		$("#host-host-name").val(data.HostName);
		$("#host-host-phone").val(data.HostPhone);
		$("#host-host-bank").val(data.HostBank);
		$("#host-host-billing-number").val(data.HostBillingNumber);
		ShowPopup($("#host-popup"));
	}
	else alert("존재하지 않는 제휴업체입니다.");
}

function UpdateCategoryCallback(model)
{
	if (model)
	{
		HidePopup($("#category-popup"));
		location.reload();
	}
	else alert("존재하지 않는 카테고리입니다.");
}
function UpdateBillingCallback(model)
{
	if (model)
	{
		HidePopup($("#billing-popup"));
		ListBilling();
	}
	else alert("존재하지 않는 입금계좌입니다.");
}
function UpdateDepartureCallback(model)
{
	if (model)
	{
		HidePopup($("#departure-popup"));
		ListDeparture();
	}
	else alert("존재하지 않는 탑승지입니다.");
}
function UpdatePriceRuleCallback(model)
{
	if (model)
	{
		HidePopup($("#price-rule-popup"));
		ListPriceRule();
	}
	else alert("존재하지 않는 가격정책입니다.");
}
function UpdateHostCallback(model)
{
	if (model)
	{
		HidePopup($("#host-popup"));
		ListHost();
	}
	else alert("존재하지 않는 제휴업체입니다.");
}

function RemoveCategoryCallback(model)
{
	if (model) location.reload();
	else alert("존재하지 않는 카테고리입니다.");
}
function RemoveBillingCallback(model)
{
	if (model) ListBilling();
	else alert("존재하지 않는 입금계좌입니다.");
}
function RemoveDepartureCallback(model)
{
	if (model) ListDeparture();
	else alert("존재하지 않는 탑승지입니다.");
}
function RemovePriceRuleCallback(model)
{
	if (model) ListPriceRule();
	else alert("존재하지 않는 가격정책입니다.");
}
function RemoveHostCallback(model)
{
	if (model) ListHost();
	else alert("존재하지 않는 제휴업체입니다.");
}

function Initialize()
{
	var header = parseFloat($("header").height()) + parseFloat($("header").css("paddingTop")) * 2;
	var collapse = $(".collapse").css("display") == "none" || $(".top-menu").css("display") != "none" ? 0 : parseFloat($(".collapse").height());
	var padding = parseFloat($("#popup-container").css("paddingTop"));

	$("article").css("top", header - collapse);
	$("#popup-container").css("top", header - collapse);
	$("#popup-container").height(window.innerHeight - header + collapse - (padding * 2));
	$(window).resize(function ()
	{
		var header = parseFloat($("header").height()) + parseFloat($("header").css("paddingTop")) * 2;
		var collapse = $(".collapse").css("display") == "none" || $(".top-menu").css("display") != "none" ? 0 : parseFloat($(".collapse").height() + 10);
		var padding = parseFloat($("#popup-container").css("paddingTop"));

		$("article").css("top", header - collapse);
		$("#popup-container").css("top", header - collapse);
		$("#popup-container").height(window.innerHeight - header + collapse - (padding * 2));
	});
	var header = parseFloat($("header").height());
	var collapse = $(".collapse").css("display") == "none" || $(window).width() >= 1200 ? 0 : parseFloat($(".collapse").height());
	var scroll = $(window).scrollTop();
	if (header - collapse <= scroll)
	{
		if ($(".collapse").css("display") == "none" || $(window).width() >= 1200)
		{
			$(".top-menu").css("background", "rgba(1, 15, 59, 0.8)");
			$(".gnb").css("background", "rgba(2, 56, 89, 0.5)");
		}
		else
		{
			$(".top-menu").css("background", "rgba(1, 15, 59, 1)");
			$(".gnb").css("background", "rgba(2, 56, 89, 1)");
		}
	}
	else
	{
		$(".top-menu").css("background", "rgba(1, 15, 59, 1)");
		$(".gnb").css("background", "rgba(2, 56, 89, 1)");
	}

	$(".go-back").on("click", function ()
	{
		window.location.href = "/Admin/Dashboard";
	});
	$(".search-bar-second .search-text-admin").on("focus", function ()
	{
		$(this).parent().css("box-shadow", "0px 0px 10px #BCE8DE");
		$(this).parent().css("border", "1px solid #BCE8DE");
		$(this).parent().find(".user-search-btn > .fa-search").css({
			"color": "#BCE8DE",
			"text-shadow": "0px 0px 10px #BCE8DE"
		});
	});
	$(".search-bar-second .search-text-admin").on("focusout", function ()
	{
		$(this).parent().css("box-shadow", "2px 2px 5px rgba(18, 18, 18, 0.3)");
		$(this).parent().css("border", "1px solid #BCE8DE");
		$(this).parent().find(".user-search-btn > .fa-search").css({
			"color": "#BCE8DE",
			"text-shadow": "none"
		});
	});
	$(".cancel-btn").on("click", function ()
	{
		var popup = $(this).parent();
		HidePopup(popup);
	});

	$("#category-keyword").on("keyup", function ()
	{
		FilterCategory();
	});
	$("#billing-keyword").on("keyup", function ()
	{
		FilterBilling();
	});
	$("#departure-keyword").on("keyup", function ()
	{
		FilterDeparture();
	});
	$("#price-rule-keyword").on("keyup", function ()
	{
		FilterPriceRule();
	});
	$("#host-keyword").on("keyup", function ()
	{
		FilterHost();
	});

	$("#add-category").on("click", function ()
	{
		ShowPopup($("#category-popup"));
	});
	$("#add-billing").on("click", function ()
	{
		ShowPopup($("#billing-popup"));
	});
	$("#add-departure").on("click", function ()
	{
		ShowPopup($("#departure-popup"));
	});
	$("#add-price-rule").on("click", function ()
	{
		ShowPopup($("#price-rule-popup"));
	});
	$("#add-host").on("click", function ()
	{
		ShowPopup($("#host-popup"));
	});

	$("#save-category").on("click", function ()
	{
		var id = parseInt($("#category-id").val());
		// id 값이 있는 경우 (수정모드)
		if (!isNaN(id))
		{
			var name = $("#category-name").val();
			var description = $("#category-description").val();
			Ajax("/Etc/UpdateCategory", {
				id: id,
				category: {
					Name: name,
					Description: description
				}
			}, UpdateCategoryCallback);
		}
		// id 값이 없는 경우 (생성모드)
		else
		{
			var name = $("#category-name").val();
			var description = $("#category-description").val();
			Ajax("/Etc/AddCategory", {
				Name: name,
				Description: description
			}, AddCategoryCallback);
		}
	});
	$("#save-billing").on("click", function ()
	{
		var id = parseInt($("#billing-id").val());
		// id 값이 있는 경우 (수정모드)
		if (!isNaN(id))
		{
			var name = $("#billing-name").val();
			var bank = $("#billing-bank").val();
			var number = $("#billing-number").val();
			Ajax("/Etc/UpdateBilling", {
				id: id,
				billing: {
					Name: name,
					Bank: bank,
					Number: number
				}
			}, UpdateBillingCallback);
		}
		// id 값이 없는 경우 (생성모드)
		else
		{
			var name = $("#billing-name").val();
			var bank = $("#billing-bank").val();
			var number = $("#billing-number").val();
			Ajax("/Etc/AddBilling", {
				Name: name,
				Bank: bank,
				Number: number
			}, AddBillingCallback);
		}
	});
	$("#save-departure").on("click", function ()
	{
		var id = parseInt($("#departure-id").val());
		// id 값이 있는 경우 (수정모드)
		if (!isNaN(id))
		{
			var name = $("#departure-name").val();
			var latitude = $("#departure-latitude").val();
			var longitude = $("#departure-longitude").val();
			Ajax("/Etc/UpdateDeparture", {
				id: id,
				departure: {
					Name: name,
					Latitude: latitude,
					Longitude: longitude
				}
			}, UpdateDepartureCallback);
		}
		// id 값이 없는 경우 (생성모드)
		else
		{
			var name = $("#departure-name").val();
			var latitude = $("#departure-latitude").val();
			var longitude = $("#departure-longitude").val();
			Ajax("/Etc/AddDeparture", {
				Name: name,
				Latitude: latitude,
				Longitude: longitude
			}, AddDepartureCallback);
		}
	});
	$("#save-price-rule").on("click", function ()
	{
		var id = parseInt($("#price-rule-id").val());
		// id 값이 있는 경우 (수정모드)
		if (!isNaN(id))
		{
			var ruleType = $("#popup-price-rule-type").val();
			var ruleName = $("#price-rule-name").val();
			var description = $("#price-rule-description").val();
			var price = $("#price-rule-price").val();
			Ajax("/Etc/UpdatePriceRule", {
				id: id,
				pricerule: {
					RuleType: ruleType,
					RuleName: ruleName,
					Description: description,
					Price: price
				}
			}, UpdatePriceRuleCallback);
		}
		// id 값이 없는 경우 (생성모드)
		else
		{
			var ruleType = $("#popup-price-rule-type").val();
			var ruleName = $("#price-rule-name").val();
			var description = $("#price-rule-description").val();
			var price = $("#price-rule-price").val();
			Ajax("/Etc/AddPriceRule", {
				RuleType: ruleType,
				RuleName: ruleName,
				Description: description,
				Price: price
			}, AddPriceRuleCallback);
		}
	});
	$("#save-host").on("click", function ()
	{
		var id = parseInt($("#host-id").val());
		// id 값이 있는 경우 (수정모드)
		if (!isNaN(id))
		{
			var type = $("#popup-host-type").val();
			var name = $("#host-name").val();
			var address = $("#host-address").val();
			var hostName = $("#host-host-name").val();
			var hostPhone = $("#host-host-phone").val();
			var hostBank = $("#host-host-bank").val();
			var hostBillingNumber = $("#host-host-billing-number").val();
			Ajax("/Etc/UpdateHost", {
				id: id,
				host: {
					Type: type,
					Name: name,
					Address: address,
					HostName: hostName,
					HostPhone: hostPhone,
					HostBank: hostBank,
					HostBillingNumber: hostBillingNumber
				}
			}, UpdateHostCallback);
		}
		// id 값이 없는 경우 (생성모드)
		else
		{
			var type = $("#popup-host-type").val();
			var name = $("#host-name").val();
			var address = $("#host-address").val();
			var hostName = $("#host-host-name").val();
			var hostPhone = $("#host-host-phone").val();
			var hostBank = $("#host-host-bank").val();
			var hostBillingNumber = $("#host-host-billing-number").val();
			Ajax("/Etc/AddHost", {
				Type: type,
				Name: name,
				Address: address,
				HostName: hostName,
				HostPhone: hostPhone,
				HostBank: hostBank,
				HostBillingNumber: hostBillingNumber
			}, AddHostCallback);
		}
	});

	$("#host-address").on("focus", function ()
	{
		if (!isPopupOpened)
		{
			new daum.Postcode({
				oncomplete: function (result)
				{
					$("#host-address").val(result.address);
				},
				onclose: function ()
				{
					isPopupOpened = false;
					$("#host-host-name").focus();
				}
			}).open();
			isPopupOpened = true;
		}
	});

}

function ShowPopup(popup)
{
	$(popup).parent().fadeIn(DURATION);
	$(popup).parent().css("display", "flex");
	$(popup).fadeIn(DURATION);
}
function HidePopup(popup)
{
	$(popup).find(".id-field").val("");
	$(popup).find(".form-control").each(function (_, item)
	{
		if (!$(item).is("select")) $(item).val("");
		else $(item).val($(item).find("option:first-child").val());
	});
	$(popup).fadeOut(DURATION);
	$(popup).parent().fadeOut(DURATION);
}

$(document).ready(function ()
{
	Initialize();
	ListCategory();
	ListBilling();
	ListDeparture();
	ListPriceRule();
	ListHost();
});