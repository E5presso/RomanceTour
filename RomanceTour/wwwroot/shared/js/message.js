$(document).ready(function ()
{
	$(".send-message").on("click", function ()
	{
		var options = `width=${screen.width / 4}, height=${screen.height / 2}, status=no, menubar=no, toolbar=no, resizable=no`;
		window.open(`/Admin/SendMessage`, "문자발송", options);
	});
});