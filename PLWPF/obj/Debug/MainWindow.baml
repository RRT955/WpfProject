   M S B A M L   `   `   `  ����@  <PLWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:PLWPFPLWPF  5   	       PLWPF.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:PLWPF     5   	     � Title$  
MainWindow��5   	   $��807.608��6   $��1275.07��6-   .��5	      � .+�5
   
   ]� $
��0, 0q�6   $	��Top=�6"   $
��auto��67   $	��Top=�6E   .+�5      o� $
��0, 0q�6   $	��Top=�6!   $��18(�66   .@�5      m� $��
Operations��6   .@�5      m� -   5         m�Name$ addTesterItem��5      $��
Add tester��66   6J   m� 5      -   5        $ addTestItem��5      $��Add test��64   6F   m� 5      -   5        $ addTraineeItem��5      $��Add trainee��67   6L   �� 5      m� 5      -   5        $ updateTesterItem��5      $��Update selected tester��69   6Y   m� 5      -   5        $ updateTestItem��5      $��Update selected test��67   6U   m� 5      -   5        $ updateTraineeItem��5      $��Update selected trainee��6:   6[   �� 5      m� 5      -   5        $ removeTesterItem��5      $��Remove selected tester��69   6Y   m� 5      -   5        $ removeTestItem��5      $��Remove selected test��67   6U   m� 5      -	   5        $ removeTraineeItem��5      $��Remove selected trainee��6:   6[   5      m� 5      $��Run a query��6   .@�5      m� -
   5        $ futureTestsItem��5      $��All future tests��68   6R   5      m� 5      $��
View items��6   .@�5      m� -   5        $ viewTesterMenuItem��5      $��View selcted tester��6;   6X   m� 5      -   5        $ viewTestMenuItem��5      $��View selcted test��69   6T   m� 5      -   5        $ viewTraineeMenuItem��5      $��View selcted trainee��6<   6Z   5       5!      5"      �� 5$   
   $��Testers:��6   $
��Left=�6#   $��	68,52,0,0q�6>   $	��Top=�6Q   $��16(�6i   &   System.Windows.Controls.DataGrid  5%   
   -   5          Name$ testersDataGrid��5%         
IsReadOnly .    AutoGenerateColumns .  $
��Left=�6Z   $
��auto��6u   $��	68,88,0,0q�6�   $	��Top=�6�   $
��auto��6�   $	��150��6�   $��14(�6�   ��6�   �� *	Testers+   Columns 5&      0  *System.Windows.Controls.DataGridTextColumn  5'      ,  &System.Windows.Controls.DataGridColumn   Header$ 	Tetser Id��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 68   �� *Id+6Q     5(      $ 
First name��6%    69   �� *	FirstName+6Y     5)      $ 	Last name��6%    68   �� *
LastName+6W     5*      $ Date of birth��6%    6<   �� *DateOfBirth+6^     5+      $ Gender��6%    65   �� *Gender+6R     5,      $ Phone number��6%    6;   �� *PhoneNumber+6]     5-      $ Address��6%    66   �� *Address.AddressAsString+6d     5.      $ Years of experience��6%    6B   �� *YearsOfExperience+6j     5/      $ Maximum weekly tests��6%    6C   �� *MaximumPossibleWeeklyTests+6t     50      $ Car type��6%    67   �� *	CarType+6U     51      $ Max distance��6%    6;   �� *MaxDistanceOfTestLocation+6k   53      �� 55   
   $��Tests:��6   $
��Left=�6!   $��
68,261,0,0q�6<   $	��Top=�6P   $��16(�6h   $��63��6v     56   
   -   5        $ testsDataGrid��56       .  .  $
��Left=�6X   $
��auto��6s   $��
68,297,0,0q�6�   $	��Top=�6�   $
��auto��6�   $	��150��6�   $��14(�6�   ��6�   �� *Tests+ 57        58      $ Test Id��6%    66   �� *TestId+6S     59      $ 
Trainee Id��6%    69   �� *	TraineeId+6Y     5:      $ 	Tetser Id��6%    68   �� *
TesterId+6W     5;      $ Date of test��6%    6;   �� *
DateOfTest+6\     5<      $ Test starting address��6%    6D   �� *&$TestDepartureAddress.AddressAsString+6     5=      $ Finished��6%    67   �� *
Finished+6V     5>      $ Passed��6%    65   �� *Passed+6R   5@      �� 5A   
   $��	Trainees:��6   $
��Left=�6$   $��
68,470,0,0q�6?   $	��Top=�6S   $��16(�6k     5B   
   -   5        $ traineesDataGrid��5B      $
��Left=�6-   $��
68,506,0,0q�6H   $	��Top=�6\   $
��1052��6t    .  .  $��14(�6�   $
��auto��6�   ��6�   �� *
Trainees+ 5C        5D      $ 
Trainee Id��6%    69   �� *Id+6R     5E      $ 
First Name��6%    69   �� *	FirstName+6Y     5F      $ 	Last Name��6%    68   �� *
LastName+6W     5G      $ Date of birth��6%    6<   �� *DateOfBirth+6^     5H      $ Address��6%    66   �� *Address.AddressAsString+6d     5I      $ Car type��6%    67   �� *	CarType+6U     5J      $ Gearbox type��6%    6;   �� *GearboxType+6]     5K      $ Driving school name��6%    6B   �� *DrivingSchoolName+6j     5L      $ Number of driving lessons��6%    6H   �� *NumberOfDrivingLessons+6u     5M      $	 Age��6%    62   �� *Age+6L     5N      $ Teacher name��6%    6;   �� *TeacherName+6]     5O      $ Gender��6%    65   �� *Gender+6R     5P      $ PhoneNumber��6%    6:   �� *PhoneNumber+6\   5R      �� 5S   
   $����Click on a tester or trainee to view them, Click on a test to update it (just when finished). or select operation from the menu.��6   $
��Left=�6�   $��
135,30,0,0q�6�   $	��Top=�6�   $	��883��6�   $��16(�6�   5T      5U      