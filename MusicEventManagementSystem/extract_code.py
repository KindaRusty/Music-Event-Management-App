# -*- coding: utf-8 -*-
import os
from pathlib import Path
from datetime import datetime

# Danh sách thư mục cần extract
FOLDERS = [
    r"C:\Users\LENOVO\Desktop\NETandRazor\App\Music-Event-Management-App\MusicEventManagementSystem\Areas\Identity",
    r"C:\Users\LENOVO\Desktop\NETandRazor\App\Music-Event-Management-App\MusicEventManagementSystem\Data",
    r"C:\Users\LENOVO\Desktop\NETandRazor\App\Music-Event-Management-App\MusicEventManagementSystem\EmailTemplates",
    r"C:\Users\LENOVO\Desktop\NETandRazor\App\Music-Event-Management-App\MusicEventManagementSystem\Migrations",
    r"C:\Users\LENOVO\Desktop\NETandRazor\App\Music-Event-Management-App\MusicEventManagementSystem\Models",
    r"C:\Users\LENOVO\Desktop\NETandRazor\App\Music-Event-Management-App\MusicEventManagementSystem\Pages",
    r"C:\Users\LENOVO\Desktop\NETandRazor\App\Music-Event-Management-App\MusicEventManagementSystem\Properties",
    r"C:\Users\LENOVO\Desktop\NETandRazor\App\Music-Event-Management-App\MusicEventManagementSystem\Services",
    r"C:\Users\LENOVO\Desktop\NETandRazor\App\Music-Event-Management-App\MusicEventManagementSystem"
]

# Các extension cần extract
EXTENSIONS = {".cs",".cshtml", ".json", ".http", ".csproj", ".sql", ".js", ".tsx"}

# Output file
OUTPUT_FILE = r"C:\Users\LENOVO\Desktop\NETandRazor\App\Music-Event-Management-App\MusicEventManagementSystem\DotNETExtract.txt"

def extract_code_files():
    """Extract tất cả các file code vào một file txt"""
    
    total_files = 0
    total_lines = 0
    errors = []
    
    try:
        with open(OUTPUT_FILE, 'w', encoding='utf-8') as output:
            output.write("=" * 80 + "\n")
            output.write("CODE EXTRACTION REPORT\n")
            output.write(f"Generated: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n")
            output.write("=" * 80 + "\n\n")
            
            # Xử lý từng thư mục
            for folder_path in FOLDERS:
                if not os.path.exists(folder_path):
                    errors.append(f"Thư mục không tồn tại: {folder_path}")
                    continue
                
                output.write(f"\n{'=' * 80}\n")
                output.write(f"FOLDER: {folder_path}\n")
                output.write(f"{'=' * 80}\n\n")
                
                folder_files = 0
                folder_lines = 0
                
                # Tìm tất cả file với extension cần thiết
                for root, dirs, files in os.walk(folder_path):
                    for file in sorted(files):
                        file_ext = Path(file).suffix.lower()
                        
                        if file_ext in EXTENSIONS:
                            file_path = os.path.join(root, file)
                            relative_path = os.path.relpath(file_path, folder_path)
                            
                            try:
                                with open(file_path, 'r', encoding='utf-8', errors='ignore') as f:
                                    content = f.read()
                                    lines = len(content.splitlines())
                                    
                                    # Ghi header file
                                    output.write(f"\n{'-' * 80}\n")
                                    output.write(f"FILE: {relative_path}\n")
                                    output.write(f"SIZE: {os.path.getsize(file_path)} bytes | LINES: {lines}\n")
                                    output.write(f"{'-' * 80}\n")
                                    output.write(content)
                                    output.write("\n\n")
                                    
                                    folder_files += 1
                                    folder_lines += lines
                                    total_files += 1
                                    total_lines += lines
                                    
                            except Exception as e:
                                error_msg = f"Lỗi đọc file {relative_path}: {str(e)}"
                                errors.append(error_msg)
                                output.write(f"\n[ERROR] {error_msg}\n\n")
                
                output.write(f"\nFolder Summary: {folder_files} files, {folder_lines} lines\n")
            
            # Ghi tóm tắt chung
            output.write(f"\n\n{'=' * 80}\n")
            output.write("SUMMARY\n")
            output.write(f"{'=' * 80}\n")
            output.write(f"Total Files: {total_files}\n")
            output.write(f"Total Lines: {total_lines}\n")
            
            if errors:
                output.write(f"\nErrors ({len(errors)}):\n")
                for error in errors:
                    output.write(f"  - {error}\n")
        
        print(f"✓ Extraction hoàn thành!")
        print(f"  Tổng file: {total_files}")
        print(f"  Tổng dòng code: {total_lines}")
        print(f"  Output file: {OUTPUT_FILE}")
        
        if errors:
            print(f"\n⚠ Có {len(errors)} lỗi:")
            for error in errors:
                print(f"  - {error}")
                
    except Exception as e:
        print(f"✗ Lỗi chung: {str(e)}")

def extract_individual_files():
    """Alternative: Extract mỗi thư mục vào file riêng"""
    
    for folder_path in FOLDERS:
        if not os.path.exists(folder_path):
            print(f"✗ Thư mục không tồn tại: {folder_path}")
            continue
        
        folder_name = os.path.basename(folder_path)
        output_file = rf"C:\Users\LENOVO\Desktop\Archery_{folder_name}_Extract.txt"
        
        total_files = 0
        total_lines = 0
        
        try:
            with open(output_file, 'w', encoding='utf-8') as output:
                output.write("=" * 80 + "\n")
                output.write(f"FOLDER: {folder_name}\n")
                output.write(f"Generated: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n")
                output.write("=" * 80 + "\n\n")
                
                for root, dirs, files in os.walk(folder_path):
                    for file in sorted(files):
                        file_ext = Path(file).suffix.lower()
                        
                        if file_ext in EXTENSIONS:
                            file_path = os.path.join(root, file)
                            relative_path = os.path.relpath(file_path, folder_path)
                            
                            try:
                                with open(file_path, 'r', encoding='utf-8', errors='ignore') as f:
                                    content = f.read()
                                    lines = len(content.splitlines())
                                    
                                    output.write(f"\n{'-' * 80}\n")
                                    output.write(f"FILE: {relative_path}\n")
                                    output.write(f"SIZE: {os.path.getsize(file_path)} bytes | LINES: {lines}\n")
                                    output.write(f"{'-' * 80}\n")
                                    output.write(content)
                                    output.write("\n\n")
                                    
                                    total_files += 1
                                    total_lines += lines
                                    
                            except Exception as e:
                                print(f"  ✗ Lỗi: {relative_path} - {str(e)}")
            
            print(f"✓ {folder_name}: {total_files} files, {total_lines} lines → {output_file}")
            
        except Exception as e:
            print(f"✗ Lỗi xử lý {folder_name}: {str(e)}")

if __name__ == "__main__":
    print("Code Extraction Tool")
    print("=" * 80)
    print("\nChọn chế độ:")
    print("1. Extract tất cả vào 1 file (mặc định)")
    print("2. Extract mỗi thư mục vào file riêng")
    
    choice = input("\nNhập lựa chọn (1 hoặc 2): ").strip()
    
    if choice == "2":
        print("\nĐang extract từng thư mục...")
        extract_individual_files()
    else:
        print("\nĐang extract tất cả thư mục vào 1 file...")
        extract_code_files()
