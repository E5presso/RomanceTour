/**
 * @license Copyright (c) 2003-2019, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function (config) {
	// 웹 폰트 추가
	config.font_names = 'Nanum Gothic;' + config.font_names;
	config.font_names = 'Noto Sans KR;' + config.font_names;
	config.font_names = 'Nanum Myeongjo;' + config.font_names;
	config.font_names = 'Gothic A1;' + config.font_names;
	config.font_names = 'Nanum Brush Script;' + config.font_names;
	config.font_names = 'Sunflower;' + config.font_names;
	config.font_names = 'Nanum Pen Script;' + config.font_names;
	config.font_names = 'Nanum Gothic Coding;' + config.font_names;
	config.font_names = 'Yeon Sung;' + config.font_names;
	config.font_names = 'Black Han Sans;' + config.font_names;
	config.font_names = 'Noto Serif KR;' + config.font_names;
	config.font_names = 'Do Hyeon;' + config.font_names;
	config.font_names = 'Gaegu;' + config.font_names;
	config.font_names = 'Dokdo;' + config.font_names;
	config.font_names = 'Gugi;' + config.font_names;
	config.font_names = 'Jua;' + config.font_names;
	config.font_names = 'Song Myung;' + config.font_names;
	config.font_names = 'Stylish;' + config.font_names;
	config.font_names = 'Poor Story;' + config.font_names;
	config.font_names = 'Gamja Flower;' + config.font_names;
	config.font_names = 'Kirang Haerang;' + config.font_names;
	config.font_names = 'East Sea Dokdo;' + config.font_names;
	config.font_names = 'Hi Melody;' + config.font_names;
	config.font_names = 'Cute Font;' + config.font_names;
	config.font_names = 'Black And White Picture;' + config.font_names;
	config.font_names = 'Single Day;' + config.font_names;

	// 기타 설정 추가
	config.filebrowserUploadUrl = '/Product/AddFile?action=true';
};