var contacts = [];
var isSelectedAll = false;
var selectPicker;

function SendCustomMessageCallback(model)
{
	if (model)
	{
		alert("메세지 전송이 완료되었습니다.");
		window.close();
	}
	else alert("메세지 전송에 실패하였습니다.");
}

function ValidateForm()
{
	contacts = [];
	$("#message-contact > option:selected").each(function (index, item)
	{
		contacts.push($(item).val());
	});

	var subject = $("#message-subject").val();
	var content = $("#message-content").val();

	if (contacts.length > 0 &&
		subject.length > 0 &&
		content.length > 0)
		$("#send-message").prop("disabled", false);
	else $("#send-message").prop("disabled", true);
}

$(document).ready(function ()
{
	selectPicker = $("#message-contact").selectpicker({
		noneSelectedText: "선택된 연락처가 없습니다.",
		countSelectedText: "{0}개의 연락처 선택됨"
	});

	$("#message-contact").on("change", function ()
	{
		ValidateForm();
	});
	$("#message-subject").on("keyup", function ()
	{
		ValidateForm();
	});
	$("#message-content").on("keyup", function ()
	{
		ValidateForm();
	});

	$("#send-message").on("click", function ()
	{
		var subject = $("#message-subject").val();
		var content = $("#message-content").val();
		Ajax("/Admin/SendCustomMessage", {
			contacts: contacts,
			subject: subject,
			content: content
		}, SendCustomMessageCallback);
	});
	$("#select-all-users").on("click", function ()
	{
		if (!isSelectedAll)
		{
			selectPicker.selectpicker('selectAll');
			$("#select-all-users").find(".btn-text").text("선택해제");
		}
		else
		{
			selectPicker.selectpicker('deselectAll');
			$("#select-all-users").find(".btn-text").text("전체선택");
		}
		isSelectedAll = !isSelectedAll;
	});
});