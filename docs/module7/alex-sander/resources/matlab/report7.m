load train
x = y;
h = [1 zeros(1, round(Fs/10)) 0.9 zeros(1, round(Fs/30)) 0.8];
y = conv(x,h);

x_padding = [x; zeros((length(y) - length(x)),1)];
h_padding = [h zeros(1,(length(y) - length(h)))];

Y = fft(y);
X = fft(x_padding);
H = fft(h_padding);

Y_w = X.*(H).';

f = [0:2*pi/length(Y_w) : (length(Y_w)-1)*2*pi/length(Y_w)]-pi;

subplot(211)
plot(f, abs(Y_w));
axis([-pi pi 0 max(abs(Y_w))*1.1]);
xlabel('Frequency (Hz)');
ylabel('X*H');

subplot(212)
plot(f, abs(Y));
axis([-pi pi 0 max(abs(Y))*1.1]);
xlabel('Frequency (Hz)');
ylabel('Y');
