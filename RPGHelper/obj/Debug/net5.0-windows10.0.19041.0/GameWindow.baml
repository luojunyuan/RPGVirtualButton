   M S B A M L   `   `   `  ����D  @RPGHelper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:RPGHelper	RPGHelper  5   	       RPGHelper.GameWindow   5   	   \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:RPGHelper     5   	   D @ModernWpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM IModernWpf.Controls, Version=0.9.3.0, Culture=neutral, PublicKeyToken=null,ui!http://schemas.modernwpf.com/2019   5   	   -   5          � Title$  
GameWindow��5	   	    � ShowInTaskbar$ False��6    � AllowsTransparency$
 True��5
   	    � WindowStyle$
 None=�6#    � Topmost$
 True��66    �� IsPressAndHoldEnabled$ False��5   	   ��5      �� 5   
   .��5      �� -   5        $��GameView��5      ._�5   
   �� .+�5      �� -   5        $��
CtrlButton��5      $
��Ctrl��6'    �� Left$ 20��66    �� Bottom$	 120��6G   ��6[   �� *
ButtonSize+��6x   �� *
ButtonSize+6�   �� 5      -   5        $��ZButton��5      $��Z��6$   $ 20��60   $ 20��6A   ��6T   �� *
ButtonSize+��6q   �� *
ButtonSize+6�   �� 5      -   5        $��XButton��5      $��X��6$   $	 120��60   $ 20��6B   ��6U   �� *
ButtonSize+��6r   �� *
ButtonSize+6�   �� 5      -   5        $��
LeftButton��5      $	��←��6'    �� Right$	 220��63   $ 20��6F   ��6Y   �� *
ButtonSize+��6v   �� *
ButtonSize+6�   �� 5      -   5        $��	TopButton��5      $	��↑��6&   $	 120��62   $	 120��6E   ��6Y   �� *
ButtonSize+��6v   �� *
ButtonSize+6�   �� 5      -   5        $��RightButton��5      $	��→��6)   $ 20��65   $ 20��6G   ��6Z   �� *
ButtonSize+��6w   �� *
ButtonSize+6�   �� 5      -	   5        $��BottomButton��5      $	��↓��6)   $	 120��65   $ 20��6H   ��6[   �� *
ButtonSize+��6x   �� *
ButtonSize+6�   �� 5      -
   5        $��	ESCButton��5      $	��ESC��6&   $ 20��64   
	 �� Top$	 20��6E   ��6U   �� *
ButtonSize+��6r   �� *
ButtonSize+6�   �� 5      -   5        $��FullScreenButton��5      $	 120��6-   $	 20��6?   ��6O   �� *
ButtonSize+��6l   �� *
ButtonSize+.��5      #  ModernWpf.Controls.SymbolIcon  
   Symbol$
 
FullScreen=�6    5       5!      5"      5#      