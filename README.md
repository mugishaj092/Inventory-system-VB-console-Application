# 📋 Inventory Management System

![Console Application](https://img.shields.io/badge/Type-Console_Application-blue)
![VB.NET](https://img.shields.io/badge/Language-VB.NET-purple)
![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-blue)

A comprehensive inventory management system with billing capabilities, built as a console application using VB.NET and PostgreSQL.

## 🌟 Features

### 📦 Inventory Management
- View all inventory items
- View detailed item information
- Add new items to inventory
- Update existing item details
- Remove items from inventory

### 🧾 Billing System
- Create new bills with multiple items
- View all billing records
- Delete existing bills
- Automatic stock deduction when creating bills

### 👤 User Management
- Secure user authentication
- New user registration
- Account profile management
- Password change functionality

## 🛠️ Technical Stack

- **Language**: VB.NET
- **Database**: PostgreSQL
- **Database Connector**: Npgsql
- **UI**: Console-based with enhanced formatting

## 📋 Database Schema

```sql
users (id, username, password, first_name, last_name, email)
items (id, name, quantity, price)
bills (id, customer_name, total_amount, created_at)
bill_items (id, bill_id, item_id, quantity, unit_price)
