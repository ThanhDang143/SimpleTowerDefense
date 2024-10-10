# Simple Tower Defense
1. 
Game có các thành phần chính là:
 - Map: được cấu tạo từ nhiều cell. Các cell này chứa các thông tin chính như loại cell (`MapCellType`), tower được đăt trên cell...
 - Tower: các trụ có thể được đặt trên map. Tấn công khi có các mục tiêu trong tầm đánh.
 - Enemy: sinh ra tại Start_Point bằng hệ thống `EnemySpawner`. Di chuyển bằng cách kiểm tra các cell có thể di chuyển ở xung quanh. Gây sát thương khi đi đến End_point. 
 - Projectile: Projectile được sinh ra từ các Tower. Và di chuyển tới target với tốc độ gấp 5 lần tốc độ của target.

Các lớp `TowerController`, `EnemyController`, `Projectile` trong project này có logic đơn giản chưa bao gồm nhiều biến thể nên chưa được thiết kế thành các abstract class.

2. 
- Các tower được đặt trong các cell trên map.
- Enemy di chuyển bằng cách kiểm tra các cell xung quanh để tìm ra điểm tiếp theo có thể di chuyển.\
- Hiện tại game chỉ hỗ trợ 1 `Start_Point` và 1 `End_Point` cũng như chưa hỗ trợ di chuyển giữa các ngã ba, ngã tư...

3.
- Nếu muốn mở rộng về chiều sâu (thêm các tính năng đặc biệt như skill, logic di chuyển...) thì có thể sửa đổi hoặc kế thừa các class như `TowerController`, `EnemyController`, `Projectile`...
- Nếu muốn mở rộng về chiều rộng (thêm enemy mới, map mới... chỉ thay đổi về chỉ số) thì có thể cập nhật dữ liệu trong file excel (Sẽ được đề cập bên dưới).

## How to play
- Hiện tại game có 2 level, khi chơi hết 2 level mà nhấn `NEXT` thì game tiếp tục load lại ở Level cuối cùng (Level 2).
- Gameplay có 2 giai đoạn chính `PREPARE`, `PLAYING`
    - Giai đoạn PREPARE không giới hạn thời gian. Không có enemy được sinh ra người chơi có thể đặt, bán tower...
    - Giai đoạn PLAYING bắt đầu khi người chơi ấn `READY`. Trong giai đoạn này quái bắt đầu được sinh ra. Người chơi vẫn có thể mua, bán trụ bình thường.
- Điều khiển:
    - Sử dụng `ASDW` hoặc các phím mũi tên để di chuyển Camera.
    - Sử dụng con lăn chuột để ZoomIn/ZoomOut map.
    - Chọn trụ cần đặt bằng cách chọn các nút trên màn hình. `Left Click` vào các cell trống để đặt trụ. `Left Click` và các cell có trụ cùng loại để nâng cấp trụ.
    - Chọn `X` ở góc dưới bên trái màn hình để huỷ chọn trụ.
    - Chọn `C` ở góc dưới bên trái màn hình để cheat thêm tiền.
    - Để bán trụ `Right Click` vào các cell có trụ. Giá bán bằng 1/2 giá của trụ.
- **Game chưa hỗ trợ save game**

## Tổng quan Unity Project.
- Bắt đầu project từ `SceneManager`.
- Chỉnh sửa data game bằng cách thêm dữ liệu vào các file excel trong folder `Assets/PluginPackages/BakingSheetImpl/RawData`. Sau khi thêm mở `Tools/DataManager` chọn `Update Data` để cập nhật data mới.
- Thêm thông qua `Tools/MapCreator`. Chọn CellType rồi click vào từng Cell trong bảng phía dưới.