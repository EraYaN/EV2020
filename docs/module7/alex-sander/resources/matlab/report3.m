load train;
p = audioplayer(y, Fs);
play(p);
get(p);

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
t = linspace(0,(length(y)/Fs),length(y)); %time axis vector t with same length as x
subplot(211)
plot(t, y)
xlabel('time [s]')
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%Fourier Transform
Y = fft(y);
w = ([0:2*pi/length(y):(length(y)-1)*2*pi/length(y)]-pi)/2*pi;

Y = fftshift(Y);
w = fftshift(w);

subplot(212)
plot(w, abs(Y));
xlabel('Frequency [Hz]')