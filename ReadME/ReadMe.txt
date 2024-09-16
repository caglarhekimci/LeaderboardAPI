//PostgreSQL için 4 gün akşamlarımı harcadım fakat düzgün yükleme işlemini, her yolu denememe rağmen yapamadım.
//Son çare olarak MySQL Redis kullanma yöntemini seçtim. Benim için de yeni bir tecrübe oldu MySQL Redis kısmı
// Hastalığım yüzünden de raporluydum ve hastanede bir süre yattım.  Gecikme için üzgünüm. Vakit ayırdığınız için teşekkürler

Dear Manager,

You can find the important steps in my project below.

Project folder structure:  

/LeaderboardAPI
│
├── /Controllers
│   ├── AuthController.cs
│   ├── LeaderboardController.cs
│
├── /Data
│   ├── AppDbContext.cs
│
│── /Migrations
│   ├── 20240915203749_InitialCreate.cs
│   ├── AppDbContextModelSnapshot.cs
│
├── /DTOs
│   ├── SubmitScoreDto.cs
│   ├── UserLoginDto.cs
│   ├── UserRegisterDto.cs
│
├── /Models
│   ├── LeaderboardEntry.cs
│   ├── MatchResult.cs
│   ├── ServiceResponse.cs
│   ├── User.cs
│
├── /Services
│   ├── AuthService.cs
│   ├── LeaderboardService.cs
│
├── ReadME
│
├── appsettings.json
├── LeaderboardAPI.http
├── Program.cs




In this N-tier architecture, the application is divided into distinct layers, each with specific responsibilities.

Controllers/ (Presentation Layer):
Files like AuthController.cs and LeaderboardController.cs are controllers responsible for handling incoming HTTP requests and directing them to the appropriate business logic. This layer interacts with the user inputs and requests.

Models/ (Data Layer):
User.cs and MatchResult.cs represent the structure of the data used in the application. This layer defines the entities or data transfer objects that interact with the database or are passed between layers.

Data/ (Data Access Layer):
AppDbContext.cs usually represents access to the database, often through an ORM like Entity Framework Core. It manages database connections and queries, abstracting the database interactions.

Services/ (Business Logic Layer):
AuthService.cs and LeaderboardService.cs contain the business logic. This layer processes data, applying business rules and logic. The controllers interact with this layer to fetch or manipulate data according to the application's business needs.

DTOs (Data Transfer Objects)
Veritabanı modelleri ile API istek/cevapları arasında veri taşıyan objeler. Bunlar veritabanı modellerinden farklı olarak sadece dış dünyaya açılan API isteklerinde kullanılır.
UserRegisterDto.cs: Kayıt olurken kullanılan kullanıcı bilgilerini içerir.
UserLoginDto.cs: Giriş için gerekli kullanıcı adı ve şifre bilgilerini içerir.
SubmitScoreDto.cs: Skor gönderimi için gerekli bilgileri taşır.




//MySQL veritabanında tabloları oluşturmak için Entity Framework Core'un migrasyon komutlarını kullanıldı.

//dotnet add package Microsoft.EntityFrameworkCore.Design
//dotnet add package Pomelo.EntityFrameworkCore.MySql
//dotnet build
//dotnet ef migrations add InitialCreate
//dotnet ef database update
Done.

Redis'i projeye entegre etme :
dotnet add package StackExchange.Redis
Then type 'PING' and wait for the 'PONG' :)


SORULAR VE YANITLAR : 
1- Explain how the system will scale under heavy traffic
2- Suggest performance improvements for MySQL, such as indexing or 
batch processing
3- Describe methods to prevent data loss during score updates in cases of
network issues or database failures
4- Explain how to manage data consistency with Redis, especially when Redis
crashes or restarts.
5- What security measures will you use to prevent score manipulation? (e.g.,
secure API calls, data encryption).
6- What logging strategies will you use to ensure the
traceability of all actions within the scoring system? How will you verify the integrity
of data stored in Redis?

YANITLAR : 
1- Sistem yoğun trafik altında ölçeklenirken Redis ve MySQL'in birlikte kullanımı önemli avantajlar sağlar:
	Redis: Sıkça erişilen veriler (örneğin liderlik tablosu) için Redis kullanarak hızlı okuma ve yazma işlemleri gerçekleştiririz. Redis bellek tabanlı bir veritabanı olduğu için yüksek trafikte hızlı erişim sağlar. Ayrıca Redis’in yatay olarak ölçeklendirilebilmesi (sharding) sayesinde yük dengeleme yapılabilir.
	MySQL: Kalıcı veriler için MySQL kullanılır. MySQL replikasyonları ve read-write splitting (okuma-yazma işlemlerini ayırma) yöntemleri kullanılarak veritabanı yükü hafifletilebilir. Trafik arttıkça MySQL sunucuları eklenerek yatay ve dikey 

2- İndeksleme: Özellikle skor, kullanıcı kimliği gibi sıkça sorgulanan sütunlar için indeksler oluşturmak, sorgu performansını önemli ölçüde artırır. Users ve MatchResults tablolarında kullanıcı kimliği ve skor alanlarına indeksler eklemek sorgu hızını artırır.
	Batch Processing: Skor güncellemelerini toplu işlem (batch processing) ile yapmak, veritabanına yapılan bireysel yazma işlemlerini azaltır. Örneğin, bir maç sonrasında birden fazla skoru tek seferde veritabanına yazmak performansı artırır. Bu şekilde, ağ ve veritabanı üzerindeki yükü hafifletebiliriz.

3- Veri kaybını önlemek için aşağıdaki yöntemler uygulanabilir:
	Transaction Kullanımı: MySQL'de skor güncellemeleri sırasında ACID özelliklerini sağlayan transaction yapıları kullanılabilir. Bu sayede bir işlem sırasında hata olursa, tüm işlem geri alınır ve veri tutarlılığı korunur.
	Retry Mekanizması: Ağ sorunları veya geçici veritabanı hatalarında veri kaybını önlemek için, başarısız olan işlemleri tekrar denemek üzere retry mekanizması kullanılabilir. Bu mekanizma, hatalı işlemleri belirli aralıklarla tekrar deneyerek veri kaybını engeller.

4- Redis, bellek tabanlı bir veritabanı olduğu için yeniden başlatıldığında veya çöktüğünde veriler kaybolabilir. Bunu önlemek için aşağıdaki yöntemler kullanılabilir:
	AOF (Append Only File): Redis’te Append Only File modu etkinleştirilerek her yazma işlemi diske kaydedilir. Bu sayede Redis yeniden başlatıldığında bu dosyadan veriler geri yüklenebilir. Ancak, bu performansı bir miktar düşürebilir. (appendonly yes)
	Snapshot (RDB): Redis belirli aralıklarla snapshot (anlık görüntü) alabilir. Eğer Redis çökerse, en son snapshot’tan veriler geri yüklenebilir. Bu, veri kaybını en aza indirir. (save 900 1  # Her 900 saniyede en az 1 değişiklik varsa snapshot alınır)
	Data Sync: Redis çöktüğünde veriler MySQL'den yeniden yüklenebilir. Redis, cache olarak kullanıldığından, veri kaybı durumunda kalıcı veritabanı olan MySQL’den veriler geri getirilebilir.

5- API Güvenliği (JWT Token): Skor güncellemeleri gibi hassas işlemler için tüm API çağrıları güvenli bir şekilde yapılmalıdır. Bunun için JWT (JSON Web Token) kullanarak kullanıcıların kimlik doğrulaması yapılır ve her API çağrısı yetkilendirilir. Bu sayede yalnızca yetkili kullanıcılar skorları güncelleyebilir.
	Veri Şifreleme: Hassas veriler (örneğin kullanıcı şifreleri) MySQL'de saklanmadan önce şifrelenmelidir. Bunun için BCrypt gibi güçlü bir şifreleme algoritması kullanılır. Böylece veri tabanına sızılması durumunda bile şifreler güvenli kalır.
	Rate Limiting: Aynı kullanıcı tarafından aşırı miktarda skor güncelleme isteğini önlemek için rate limiting uygulanabilir. Bu, kötü niyetli kullanıcıların sistemde manipülasyon yapmasını önler.

6- Loglama Stratejisi: Sistemdeki her kritik işlem (kullanıcı kaydı, skor güncellemesi, oturum açma) loglanmalıdır. Serilog veya NLog gibi loglama kütüphaneleri kullanılarak API çağrıları, veritabanı işlemleri ve Redis işlemleri loglanabilir.
Loglar şu bilgileri içerebilir:
- Kullanıcı ID’si
- İlgili API endpoint
- Zaman damgası
- İşlem sonucu (başarılı veya hata)

Fakat ben kullanmadım. Teslim sürecimi en kısa zamanda yaptığımdan emin olabilirsiniz. Projenin daha bir çok eksiği elbette var. ReadMe dosyasını da ingilizce oluşturmak çok isterdim fakat zamandan kazanabilmem için Türkçe devam etmek zorunda kaldım.

Vakit ayırdığınız için teşekkürler,
İyi günler dilerim.

Çağlar Hekimci
caglarhekimci@gmail.com
+905071612930