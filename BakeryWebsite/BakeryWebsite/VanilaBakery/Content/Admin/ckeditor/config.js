/// <reference path="../ckfinder/core/connector/aspx/connector.aspx" />
/// <reference path="../ckfinder/ckfinder.html" />
/// <reference path="../ckfinder/ckfinder.html" />
/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here.
	// For the complete reference:
    config.syntaxhighlight_lang = 'csharp';
    config.syntaxhighlight_hideControls = true;
    config.languages = 'vi';
    config.filebrowserBrowseUrl = '/Content/Admin/ckfinder/ckfinder.html';
    config.filebrowserImageBrowseUrl = '/Content/Admin/ckfinder/ckfinder.html?Types=Images';
    config.filebrowserFlashBrowseUrl = '/Content/Admin/ckfinder/ckfinder.html?Types=Flash';
    config.filebrowserUploadUrl = '/Content/Admin/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=File';
    config.filebrowserImageUploadUrl = '/Content/Admin/Data';
    config.filebrowserFlashUploadUrl = '/Content/Admin/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Flash';

    CKFinder.setupCKEditor(null, '/Content//Admin/ckfinder/');
};
