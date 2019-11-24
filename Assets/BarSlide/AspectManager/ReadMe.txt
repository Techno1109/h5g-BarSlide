#TinyUnity向けの実行環境の画面比率に対してUIを対応させるためのスクリプト群です

#使い方
①SystemとTagフォルダ内の.csをそれぞれインポートしてください。
②アスペクト比を対応させたいCanvasに「AspectManageTaeger」をアタッチしてください。
※必ず、Canvasの「UIScaleMode」を『ScaleWithScreenSize』に指定してください。
　この設定を怠った場合、正しく機能しません。

#おまけ機能
おまけとしてアスペクト比率を取得できる関数を定義しています。
利用する際は「Liberapp.Ui.AspectManager」をUsingしてください。