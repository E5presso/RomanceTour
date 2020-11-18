var isPopupOpened = false;
var UserStatus =
{
	GREEN: "GREEN",
	YELLOW: "YELLOW",
	RED: "RED",
	GREY: "GREY"
};

function FilterProduct()
{
	Ajax("/User/ListUser", {FilterProductCallbackack);
}

functiFilterProductCallbackack(model)
{
	$(".user-list").html('');
	$.each(model, function (index, item)
	{
		$(".user-list").append(`
			<tr id="user-${item.Id}">
				<td class="d-table-cell">
					<span class="table-body">
						${item.UserName}
					</span>
				</td>
				<td class="d-table-cell">
					<select class="table-body form-control user-status">
						<option value="${UserStatus.GREEN}" ${item.Status == UserStatus.GREEN ? "selected" : ""}>정상</option>
						<option value="${UserStatus.YELLOW}" ${item.Status == UserStatus.YELLOW ? "selected" : ""}>경고</option>
						<option value="${UserStatus.RED}" ${item.Status == UserStatus.RED ? "selected" : ""}>정지</option>
						<option value="${UserStatus.GREY}" ${item.Status == UserStatus.GREY ? "selected" : ""}>휴면</option>
					</select>
				</td>
				<td class="d-table-cell">
					<span class="table-body">
						${item.Name}
					</span>
				</td>
				<td class="d-none d-md-table-cell">
					<span class="table-body">
						${item.Phone}
					</span>
				</td>
				<td class="d-none d-lg-table-cell">
					<span class="table-body">
						${item.Address}
					</span>
				</td>
				<td class="d-none d-xl-table-cell">
					<span class="table-body">
						${new Date(item.Birthday).toLocaleDateString()}
					</span>
				</td>
				<td class="d-table-cell">
					<span class="table-body">
						<button class="btn third view-history">
							<span class="fa fa-history btn-icon"></span>
						</button>
						<button class="btn third config-user">
							<span class="fa fa-user-cog btn-icon"></span>
						</button>
						<button class="btn third remove-user">
							<span class="fa fa-trash btn-icon"></span>
						</button>
					</span>
				</td>
			</tr>
		`);
		$(`#user-${item.Id} .user-status`).on("change", function ()
		{
			var id = $(this).parent().parent().attr("id").split("-")[1];
			var status = $(this).val();
			Ajax("/User/AdminUpdateUserStatus", {
				id: id,
				status: status
			}, AdminUpdateUserStatusCallback);
		});
		$(`#user-${item.Id} .view-history`).on("click", function ()
		{
			var id = $(this).parent().parent().parent().attr("id").split("-")[1];
			Ajax("/User/AdminGetUserHistory", {
				id: id
			}, AdminGetUserHistoryCallback);
		});
		$(`#user-${item.Id} .config-user`).on("click", function ()
		{
			var id = $(this).parent().parent().parent().attr("id").split("-")[1];
			Ajax("/User/AdminGetUser", {
				id: id
			}, AdminGetUserCallback);
			ShowUserInfoPopup();
		});
		$(`#user-${item.Id} .remove-user`).on("click", function ()
		{
			if (confirm("정말로 해당 사용자를 제거하시겠습니까?\n이 동작은 취소할 수 없습니다."))
			{
				var id = $(this).parent().parent().parent().attr("id").split("-")[1];
				Ajax("/User/RemoveUser", {
					id: id
				}, RemoveUserCallback);
			}
		});
	});
}
function AdminGetUserCallback(model)
{
	if (model.Result)
	{
		var user = model.Data;
		$("#user-id").val(user.Id);
		$("#user-username").val(user.UserName);
		$("#user-name").val(user.Name);
		$("#user-phone").val(user.Phone);
		$("#user-birthday").val(new Date(user.Birthday).format("yyyy-MM-dd"));
		$("#user-address").val(user.Address);
		$("#user-billing-name").val(user.BillingName);
		$("#user-billing-bank").val(user.BillingBank);
		$("#user-billing-number").val(user.BillingNumber);
	}
	else
	{
		alert(model.Message);
		FilterProduct();
	}
}
function AdminGetUserHistoryCallback(model)
{
	if (model.Result)
	{
		var logs = model.Data;
		var message = "";
		var error = $("#error-action").val();
		var accessDenied = $("#access-denied-action").val();
		var pageNotFound = $("#page-not-found-action").val();
		$.each(logs, function (index, item)
		{
			if (item.Action == error || item.Action == accessDenied || item.Action == pageNotFound)
				message += `<span class="log-message something-suspicious">[${new Date(item.TimeStamp).format("yyyy-MM-dd HH:mm:ss")}] ${item.IpAddress}에서 ${item.Action}에 접근하였습니다.</span>\n`;
			else
				message += `<span class="log-message">[${new Date(item.TimeStamp).format("yyyy-MM-dd HH:mm:ss")}] ${item.IpAddress}에서 ${item.Controller}의 ${item.Action}에 접근하였습니다.</span>\n`;
		});
		$("#user-history").html(message);
		ShowUserHistoryPopup();
	}
	else
	{
		alert(model.Message);
		FilterProduct();
	}
}
function AdminUpdateUserCallback(model)
{
	if (model.Result)
	{
		alert(model.Message);
		HideUserInfoPopup();
		FilterProduct();
	}
	else
	{
		alert(model.Message);
		FilterProduct();
	}
}
function AdminUpdateUserStatusCallback(model)
{
	if (model.Result)
	{
		FilterProduct();
	}
	else
	{
		alert(model.Message);
		FilterProduct();
	}
}
function RemoveUserCallback(model)
{
	if (model) alert("사용자가 정상적으로 제거되었습니다.");
	else alert("사용자 제거에 실패하였습니다.");
	Ajax("FilterProductCallback}, FilterUserCallback);
}

function ShowUserInfoPopup()
{
	$("#popup-container").fadeIn(DURATION);
	$("#popup-container").css("display", "flex");
	$("#popup-user-info").fadeIn(DURATION);
}
function HideUserInfoPopup()
{
	$("#user-username").val("");
	$("#user-name").val("");
	$("#user-phone").val("");
	$("#user-address").val("");
	$("#user-birthday").val("");
	$("#user-billing-name").val("");
	$("#user-billing-bank").val(0);
	$("#user-billing-number").val("");
	$("#popup-user-info").fadeOut(DURATION);
	$("#popup-container").fadeOut(DURATION);
}

function ShowUserHistoryPopup()
{
	$("#popup-container").fadeIn(DURATION);
	$("#popup-container").css("display", "flex");
	$("#popup-user-history").fadeIn(DURATION);
}
function HideUserHistoryPopup()
{
	$("#user-history").val("");
	$("#popup-user-history").fadeOut(DURATION);
	$("#popup-container").fadeOut(DURATION);
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
	$(".search-bar-second .search-text").on("focus", function ()
	{
		$(this).parent().css("box-shadow", "0px 0px 10px #B0C8EB");
		$(this).parent().css("border", "1px solid #B0C8EB");
		$(this).parent().find(".user-search-btn > .fa-search").css({
			"color": "#B0C8EB",
			"text-shadow": "0px 0px 10px #B0C8EB"
		});
	});
	$(".search-bar-second .search-text").on("focusout", function ()
	{
		$(this).parent().css("box-shadow", "initial");
		$(this).parent().css("border", "1px solid #B0C8EB");
		$(this).parent().find(".user-search-btn > .fa-search").css({
			"color": "#B0C8EB",
			"text-shadow": "none"
		});
	});
	$(".search-bar-second .search-text").on("keyup", function ()
	{
		var option = $(".search-option option:selected").val();
		var keyword = $(this).val();
		AjaxWithoutLoading("/User/SearchUser", {
			option: option,
			keFilterProductCallback}, FilterUserCallback);
	});

	$("#user-address").on("focus", function ()
	{
		if (!isPopupOpened)
		{
			new daum.Postcode({
				oncomplete: function (result)
				{
					$("#user-address").val(result.address);
				},
				onclose: function ()
				{
					isPopupOpened = false;
					$("#user-billing-name").focus();
				}
			}).open();
			isPopupOpened = true;
		}
	});

	$("#save-user-info").on("click", function ()
	{
		var id = parseInt($("#user-id").val());
		var name = $("#user-name").val();
		var phone = $("#user-phone").val();
		var address = $("#user-address").val();
		var birthday = $("#user-birthday").val();
		var billingName = $("#user-billing-name").val();
		var billingBank = $("#user-billing-bank").val()
		var billingNumber = $("#user-billing-number").val();
		Ajax("/User/AdminUpdateUser", {
			id: id,
			user: {
				Name: name,
				Phone: phone,
				Address: address,
				Birthday: birthday,
				BillingName: billingName,
				BillingBank: billingBank,
				BillingNumber: billingNumber
			}
		}, AdminUpdateUserCallback);
	});
	$("#close-popup-user-info").on("click", function ()
	{
		HideUserInfoPopup();
	});
	$("#close-popup-user-history").on("click", function ()
	{
		HideUserHistoryPopup();
	});
}

$(document).ready(function ()
{
	Initialize();
	FilterProduct();
});