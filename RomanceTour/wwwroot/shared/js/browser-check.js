function IsMSIE() 
{
	var ua = window.navigator.userAgent;
	var msie = ua.indexOf("MSIE ");

	if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) return true;
	else return false;
}

$(document).ready(function ()
{
	if (IsMSIE())
	{
		setTimeout(function ()
		{
			alert("인터넷 익스플로러 사용자는 서비스가 제한됩니다.\nEdge 브라우저를 사용하여 접속해주세요.\n이용에 불편을 드려 죄송합니다.");
			window.open('', '_self').close();
		}, 100);
	}
});