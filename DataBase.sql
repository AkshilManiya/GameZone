
CREATE TABLE tblOffer (
    Offer_id INT PRIMARY KEY IDENTITY(1,1),
    Percentage DECIMAL(5, 2)
);

CREATE TABLE tblDay (
    Day_id INT PRIMARY KEY IDENTITY(1,1),
    day_name VARCHAR(50),
    Date DATE
);

CREATE TABLE tblTimeframe (
    Time_frame_id INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(50)
);

CREATE TABLE tblLogin (
    Login_id INT PRIMARY KEY IDENTITY(1,1),
    Username VARCHAR(50),
    Password VARCHAR(100),
    Role VARCHAR(50)
);

CREATE TABLE tblState (
    State_id INT PRIMARY KEY IDENTITY(1,1),
    State_name VARCHAR(20)
);

CREATE TABLE tblCity (
    City_id INT PRIMARY KEY IDENTITY(1,1),
    State INT FOREIGN KEY REFERENCES tblState(State_id),
    City_name VARCHAR(20)
);

CREATE TABLE tblArea (
    Area_id INT PRIMARY KEY IDENTITY(1,1),
    City INT FOREIGN KEY REFERENCES tblCity(City_id),
    Area_name VARCHAR(20)
);

CREATE TABLE tblAdmin (
    Admin_id INT PRIMARY KEY IDENTITY(1,1),
    Area INT FOREIGN KEY REFERENCES tblArea(Area_id),
    Name VARCHAR(20),
    Mobile_no VARCHAR(10),
    Password VARCHAR(100),
    Email VARCHAR(50),
    Wallet DECIMAL(10, 2),
    Image VARCHAR(100)
);

CREATE TABLE tblManager (
    Manager_id INT PRIMARY KEY IDENTITY(1,1),
    Area INT FOREIGN KEY REFERENCES tblArea(Area_id),
    Name VARCHAR(20),
    Mobile_no VARCHAR(10),
    Password VARCHAR(100),
    Email VARCHAR(50),
    Wallet DECIMAL(10, 2),
    Image VARCHAR(100)
);

CREATE TABLE tblMember (
    Member_id INT PRIMARY KEY IDENTITY(1,1),
    Area INT FOREIGN KEY REFERENCES tblArea(Area_id),
    Name VARCHAR(20),
    Mobile_no VARCHAR(10),
    Password VARCHAR(100),
    Email VARCHAR(50),
    Wallet DECIMAL(10, 2),
    Image VARCHAR(100)
);

CREATE TABLE tblClub (
    Club_id INT PRIMARY KEY IDENTITY(1,1),
    Area INT FOREIGN KEY REFERENCES tblArea(Area_id),
    Manager INT FOREIGN KEY REFERENCES tblManager(Manager_id),
    Club_name VARCHAR(30),
    Address VARCHAR(100),
    Joining_date DATE,
    Image VARCHAR(100),
    Rating int,
    Club_isDeleted BIT
);



CREATE TABLE tblGame (
    Game_id INT PRIMARY KEY IDENTITY(1,1),
    Club INT FOREIGN KEY REFERENCES tblClub(Club_id),
    Offer INT FOREIGN KEY REFERENCES tblOffer(Offer_id),
    Game_name VARCHAR(20),
    Rating int,
    Members INT,
    image VARCHAR(100),
    Caption VARCHAR(100),
    Charge_method VARCHAR(50),
    State int,
    Game_isDeleted BIT
);



CREATE TABLE tblRowslot (
    Rowslot_id INT PRIMARY KEY IDENTITY(1,1),
    Game INT FOREIGN KEY REFERENCES tblGame(Game_id),
    Time_frame INT FOREIGN KEY REFERENCES tblTimeframe(Time_frame_id),
    Day INT FOREIGN KEY REFERENCES tblDay(Day_id),
    Duration TIME,
    Starting_time TIME,
    Rant DECIMAL(10, 2)
);

CREATE TABLE tblSlot (
    Slot_id INT PRIMARY KEY IDENTITY(1,1),
    Game INT FOREIGN KEY REFERENCES tblGame(Game_id),
    Time_frame INT FOREIGN KEY REFERENCES tblTimeframe(Time_frame_id),
    Date DATE,
    Duration TIME,
    Starting_time TIME,
    Rant DECIMAL(10, 2),
    State int
);

CREATE TABLE tblBookings (
    Booking_id INT PRIMARY KEY IDENTITY(1,1),
    Slot INT FOREIGN KEY REFERENCES tblSlot(Slot_id),
    Member INT FOREIGN KEY REFERENCES tblMember(Member_id),
    Date DATE,
    Players INT,
    Total_amount DECIMAL(10, 2),
    Rating int,
    Check_in BIT
);

CREATE TABLE tblPayment (
    Payment_id INT PRIMARY KEY IDENTITY(1,1),
    Login INT FOREIGN KEY REFERENCES tblLogin(Login_id),
    Amount DECIMAL(10, 2),
    Date DATETIME,
    Payment_operation VARCHAR(50),
    Game INT FOREIGN KEY REFERENCES tblGame(Game_id)
);
