# OSCTestServer

## このアプリについて
バーチャルキャストのOSC機能から送信されたOSCパケットのOSCのアドレスに応じて送信先を変えるアプリです。

またアドレス次第でSendkeysでアクティブウィンドウにキー送信します。

コンパイラはwindows標準のcsc.exeでできます。

勉強用で作ったアプリです。

汚いコードですがライセンスはMITですのでご自由にお使いください。

## 使い方
各種設定をしてstartボタンを押してください。

ポート設定を変えるときはstopボタンを押して設定を変更した後、再度startボタンを押してください。

## 識別アドレスとその挙動
### 先頭アドレスが/loopback/の場合
バーチャルキャストの受信ポートにデータを送り返します。

設定次第で先頭の/loopbackを取り除きます。
### 先頭アドレスが/outside/の場合
設定したIPアドレスにデータを送ります。

設定次第で先頭の/outsideを取り除きます。

### アドレスが/sendkeysのみの場合
データがstringだけの場合のみアクティブウィンドウにキーを送信します。

意図しないウィンドウがアクティブになっていてもそのウィンドウにキー送信することがありますので注意してください。

## 設定項目
### このアプリからの送信ポート
アプリから送信するポートです。

下記の「バーチャルキャストで設定した受信ポート」とは**別のポート**にしてください。

loopbackの場合は自動的にバーチャルキャストの受信ポートに設定します。

### バーチャルキャストで設定した送信ポート
バーチャルキャストアプリで設定した送信ポートを入力してください。

このアプリの受信ポートになります。

### バーチャルキャストで設定した受信ポート
バーチャルキャストアプリで設定した受信ポートを入力してください。

### 送信先外部IPアドレス
送信したい外部のIPアドレスを入力してください。

### アドレスから/loopbackを除く
チェックされていた場合、転送する際にアドレス先頭に/loopbackがあればそれを取り除いて転送します。

### アドレスから/outsideを除く
チェックされていた場合、転送する際にアドレス先頭に/outsideがあればそれを取り除いて転送します。
## License
[MIT License](https://github.com/teiron3/OSCTestServer/blob/main/LICENSE)
## 外部参考リンク
[バーチャルキャスト](https://virtualcast.jp/)

[Open Sound Control 1.0仕様](http://veritas-vos-liberabit.com/trans/OSC/OSC-spec-1_0.html)
