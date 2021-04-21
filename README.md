# GOLDPRIDE-FINAL
Event management web app using C# mvc and various types of technology such as Api's ,sandboxes, scanners, voice to text etc.
this application uses a voice to text feature when admin creates a new meeting 
there is google maps that is used to calculate distance and output a price if delivery is needed
emails are being sent with invoices and notifications
there is quartz.net being used as a scheduler for reminders
you have to add your own api keys for the gmaps
add your own api keys for email, use a free sendgrid account and put your keys in your environment variable
qr code scan to pick up orders
digital signatures

#Admin(username: admin@gmail.com, password: Admin1)
-View all meetings made by client
-start a new meeting and record minutes manually or via voice to text
-add new halls/products/categories
-add re add stock back by scanning qr of individual items
-assign driver to deliveries
-mark reviews as visible

#Driver(username: driver@gmail.com, password: Driver1, ID: 2)
-view all deliveries and collections
-upon arrival of delivery scan customer invoice qr and make them sign
same as collection

#Customer
-book appointments
-hire products
-book halls for events 
-pay via payfast sandbox
-view ongoing and past orders
-review product when email has een received withg the link
-have option for self collection or delivery
-google maps is used to calculate distance if delivery is chosen and a price is output

Find Screenshots of the application here- https://mega.nz/folder/IiRwmR7I#4iNWKDMIQg3yS8c1P89PLQ
