# WorkflowApp.Api

業務ワークフロー（申請・承認）を想定したWeb APIです。  
ユーザー登録・ログイン（JWT認証）機能を提供します。

## 作成背景

デスクトップアプリ中心の開発経験から、Webアプリケーション（特にAPI設計・認証・DI）へのスキル拡張を目的として作成しました。

## 技術スタック

- .NET 10.0 / ASP.NET Core Web API
- Entity Framework Core
- JWT Authentication
- xUnit（単体テスト）

## アーキテクチャ

レイヤード構成を採用しています。

- Controllers（API層）
- Services（アプリケーション層）
- Infrastructure（DB・外部依存）
- DTO

## 機能

- ユーザー登録
- ログイン（JWT発行）
- 認証付きAPI
- バリデーション

## 認証方法

1. `/api/auth/login` でJWTトークンを取得
2. リクエストヘッダーに設定

Authorization: Bearer {token}

## API一覧

| Method | Endpoint           | 説明         |
| ------ | ------------------ | ------------ |
| POST   | /api/auth/register | ユーザー登録 |
| POST   | /api/auth/login    | ログイン     |

## 実行方法

```bash
git clone https://github.com/ssakaguchi/WorkflowApp.Api.git
cd WorkflowApp.Api/WorkflowApp.Api
dotnet restore
dotnet run
```

## 認証方法

1. `/api/auth/login` でJWTトークンを取得
2. リクエストヘッダーに設定

Authorization: Bearer {token}

## テスト

xUnitを使用してAuthServiceの単体テストを実装しています。

```bash
dotnet test
```
