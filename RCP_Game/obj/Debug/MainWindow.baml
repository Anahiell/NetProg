   M S B A M L   `   `   `  ����C  ?RCP_Game, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:RCP_GameRCP_Game  5   	       RCP_Game.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:RCP_Game     5   	     � Title$  
MainWindow��5   	   $	��600��6   $	��800��6)   
��� ���.��5	      �  � ColumnDefinitions 5
   
   �� 5      �� 5      �� 5      �� 5      �� 5       � RowDefinitions 5   
   �� 5      $
��0.3* �6   �� 5      �� 5      $
��0.3* �6   �� 5      $
��0.5* �6   .+�5   
   �� -   5         ��Name$ ConnectButton��5      $��0��6D   $��0��6T   $��5q�6a   
��� � �
 � Tag$ connect��6   .��6�   	Connect6�   �� 5   
   -   5        $ StartButton��5      $��4��6@   $��0��6P   $��5q�6]   
������.��6�   Start6�   �� 5   
   -   5        $ 
RockButton��5      $��1��6%   $��3��65   $��5q�6B   $��Center=�6M   $
 rock��6�   .��5      �� $��Uniform=�6   ��5      �� 5       �� 	UriSource$K EC:\Users\user\Desktop\сетквое\NetProg\RCP_Game\Source\rock.png?�6"   5      5      �� 5!   
   -   5        $ PaperButton��5!      $��2��6&   $��3��66   $��5q�6C   $��Center=�6N   $ paper��6�   .��5"      �� $��Uniform=�6   ��5#      �� 5$      $M GC:\Users\user\Desktop\сетквое\NetProg\RCP_Game\Source\paper.webp?�6"   5&      5'      �� 5)   
   -   5        $ ScissorsButton��5)      $��3��6*   $��3��6:   $��5q�6G   $��Center=�6R   $ scissors��6�   .��5*      �� $��Uniform=�6   ��5+      �� 5,      $K EC:\Users\user\Desktop\сетквое\NetProg\RCP_Game\Source\cut.webp?�6"   5.      5/      �� 50   
   -   5        $��4��50      $��2��6!   $��15q�6.   .��6X   	surrender6c   �� 51   
   -   5         ��Name$ StatusTextBlock��51      $��1��6-   $��3��6=   $��0��6Q   $��5q�6^   
���   �
��� � �$��20(�6�    �� TextAlignment$ Center=�6�   Info6�   �� 52   
   -   5        $ InfoTextBlock��52      $��2��6+   $��1��68   $��3��6H   $��5q�6\   
����� �$��20(�6y   $ Center=�6�   Status6�   �� 53   
   -	   5         ��Name$ CenterImage��53      $��2��6%   $��1��65   $��Uniform=�6B   54      55      