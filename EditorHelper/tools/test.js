var popup = new ActiveXObject('EditorHelper.Popup');
popup.AddMenu("Test", 0);

var ret = popup.TrackMenu();

WScript.echo("Success:" + ret);
