   M S B A M L   `   `   `  ����F  BmodBusParse, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:modBusParsemodBusParse  5   	       modBusParse.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:modBusParse     5   	     � Title$  Data converter��5   	   $	��450��6    $	��800��6-   $	��800��69   $	��450��6H   $	��800��6X   $	��450��6g    � 
ResizeMode$ CanMinimize=�6w   .��5	      �  � RowDefinitions 5
   
   �� 5       � ColumnDefinitions 5   
   �� 5      $
��107* �6   �� 5      $
��157* �6   .+�5   
   �� -   5         ��Name$ modbus_logo_png��5      $��276,360,10,0q�6)   $ ��background/modbus_logo.png��6?   $
��Fill=�6c   $��1��6r   �� 5   
   -   5         ��Name$ BtnFileOpen��5      $��Browse��6&   $
��Left=�67   $��
47,336,0,0q�6R   $	��Top=�6f   $	��108��6~    J� RenderTransformOrigin$ 
0.009,0.75&�6�   � 5   
   $��276,106,67,234q�6   $��1��6'    5      �� 5      $	��86* �6    �� 5      $	��27* �6    .+�5      	� -   5         	�Name$ rb_JSON��5      $
��JSON��6+   $
��Left=�6:    d� 	IsChecked$
 TrueY�6o   $��0,0,0,41q�6�   	� 5      -   5        $ rb_XML��5      $	��XML��6*   $
��Left=�68   $��	0,20,0,21q�6S   6f   	� 5      -   5        $ rb_TXT��5      $	��TXT��6*   $��	0,40,46,1q�68   6K   5      �� 5   
   -   5        	 ��Name$	 label��5      $��Choose output format��6   $
��Left=�6>   $	��Top=�6Y   $��
276,74,0,0q�6q   $	��128��6�   $��1��6�   �� 5   
   -   5        $ btn_CreateLog��5      $��Create log from file��6(   $
��Left=�6G   $��260,187,0,0q�6b   $	��Top=�6w   $	��164��6�   $��1��6�   {�.  6�   �� 5   
   -   5        $	 label_FilePath��5      $�� ��6(   $
��Left=�63   $	��Top=�6N   $��
15,355,0,0q�6f   $	��465��6z   $��64��6�   $��2��6�   �� 5    
   -	   5        $	 label_dataField��5       $��Raw data input:��6)   $
��Left=�6C   $	��Top=�6^   $��	34,43,0,0q�6v   $	��128��6�   �� 5!   
   -
   5        $	 label_browseFile��5!      $��Browse log files��6*   $
��Left=�6E   $	��Top=�6`   $��
47,310,0,0q�6x   $	��173��6�   �� 5"   
   -   5        $ btn_CreateLogFromRawData��5"      $��Create log from raw data��63   $��1��6V   $
��Left=�6f   $��247,213,0,0q�6�   $	��Top=�6�   $	��190��6�   {�.  6�   �� 5#   
   -   5        
 ��Name$
 RichTextBoxData��5#      $
��Left=�6/   $	��236��6J   $��	34,74,0,0q�6W   $	��Top=�6j   $	��515��6�   $��2��6�   . �5$      /� 5%      5'      5(      