   M S B A M L   `   `   `  ����D  @Socket 4I, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:Socket_4I	Socket_4I  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	       Socket_4I.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:Socket_4I     5   	   9System&clr-namespace:System;assembly=mscorlib     5   	     � Title$
  Chat��5	   	   $	��369��6   $	��523��6#   .��5
      � .+�5   
   �� -   5         ��Name$ 
lstRubrica��5      $
��Left=�6&   $	��192��6A   $��	21,73,0,0q�6N   $	��Top=�6a   $	��191��6y   6�   �� 5   
   -   5        $ lstMessaggi��5      $
��Left=�6'   $	��192��6B   $��
286,73,0,0q�6O   $	��Top=�6c   $	��198��6{   �� 5   
   -   5         ��Name$ btnInvia��5      $��Invia��6#   $
��Left=�63   $��306,304,0,0q�6N   $	��Top=�6c   $��74��6{    �� 	IsDefault . 6�   �� 5   
   -   5        $ btnConfermaIpPorta��5      $��Conferma��6-   $
��Left=�6@   $��
326,12,0,0q�6[   $	��Top=�6o   $��75��6�   6�   �� 5   
   -   5        $ btnBroadcast��5      $��	Broadcast��6'   $
��Left=�6;   $��385,304,0,0q�6V   $	��Top=�6k   $��74��6�   6�   �� 5   
   -   5        $ btnAggiungiContatto��5      $��Aggiungi��6.   $
��Left=�6A   $��218,254,0,0q�6\   $	��Top=�6q   $��62��6�   6�   �� 5   
   -   5        $ btnModificaContatto��5      $��Modifica��6.   $
��Left=�6A   $��218,279,0,0q�6\   $	��Top=�6q   $��62��6�   6�   �� 5   
   -   5        $ btnEliminaContatto��5      $��Elimina��6-   $
��Left=�6?   $��218,304,0,0q�6Z   $	��Top=�6o   $��62��6�   ���	#FFFF01016�   �� 5   
   -	   5        $��txtIPLocale��5      $
��Left=�6%   $��	68,14,0,0q�6@    �� TextWrapping$
 Wrap=�6S   $��	127.0.0.1��6g   $	��Top=�6x   $	��101��6�   �� 5   
   -
   5        $��txtPortaLocale��5      $
��Left=�6(   $��
255,14,0,0q�6C   $
 Wrap=�6W   $��10000��6k   $	��Top=�6x   $��55��6�   �� 5   
   -   5         ��Name$ txtMessaggio��5      $
��Left=�6(   $��23��6C   $��306,270,0,0q�6O   $
 Wrap=�6d   $	��Top=�6x   $	��153��6�   �� 5   
   -   5        $ txtNomeContatto��5      $
��Left=�6+   $��
66,305,0,0q�6F   $
 Wrap=�6Z   $	��Top=�6n   $	��140��6�   �� 5   
   -   5        $ txtIPContatto��5      $
��Left=�6)   $��
27,275,0,0q�6D   $
 Wrap=�6X   $�� ��6l   $	��Top=�6t   $	��101��6�   �� 5   
   -   5        $ txtPortaContatto��5      $
��Left=�6,   $��151,275,0,0q�6G   $
 Wrap=�6\   $�� ��6p   $	��Top=�6x   $��55��6�   �� 5   
   $��Contatti��6   $
��Left=�6#   $��	21,41,0,0q�6>   $	��Top=�6Q   $��27��6i   �� 5   
   -   5        $��lblContatto��5      $!��Socket non inizializzata...��6#   $
��Left=�6I   $��
282,42,0,0q�6d   $	��Top=�6x   �� 5   
   $��
Il tuo IP:��6   $
��Left=�6%   $��	10,10,0,0q�6@   $	��Top=�6S   �� 5   
   $��La tua Porta:��6   $
��Left=�6(   $��
174,10,0,0q�6C   $	��Top=�6W   �� 5    
   $��Nome:��6   $
��Left=�6    $��
21,298,0,0q�6;   $	��Top=�6O   �� 5!   
   $��:��6   $
��Left=�6   $��133,269,0,0q�67   $	��Top=�6L   5#      5$      